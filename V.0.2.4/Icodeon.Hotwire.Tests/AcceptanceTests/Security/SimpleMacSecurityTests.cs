using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using FluentAssertions;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Contracts.Enums;
using Icodeon.Hotwire.Framework.DTOs;
using Icodeon.Hotwire.Framework.MediaTypes;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.DAL;
using Icodeon.Hotwire.Framework.Security;
using Icodeon.Hotwire.Framework.Utils;
using Icodeon.Hotwire.TestFramework;
using Icodeon.Hotwire.TestFramework.Mocks;
using NUnit.Framework;
using StructureMap;

namespace Icodeon.OUIntegration.Tests.AcceptanceTests.End2EndDeploys
{
    [TestFixture]
    public class SimpleMacSecurityTests : UnitTest
    {

        // SCALE: code below will require sticky sessions, i.e. users to complete requests on the same machine, alternatively 
        //        the salt cache provider will have to be a distributed cache of some kind.


        public class ValidMacRequiredIfEndpointIsSecuredRow : RestScenario
        {
            public bool MacHeaderProvided { get; set; }
            public bool EndpointSecuredWithSimpleMac { get; set; }
            public bool CorrectKeyUsedToSign { get; set; }
            public bool Signed { get; set; }
        }


        [Test]
        public void ValidMacRequiredIfEndpointIsSecured()
        {
            TraceTitle("ValidMacRequiredIfEndpointIsSecured()");
            string story = @"

            |Title                                              |MacHeaderProvided |EndpointSecuredWithSimpleMac |Signed | CorrectKeyUsedToSign | Response    | ResponseTextToContain               |#
            |---------------------------------------------------|------------------|-----------------------------|-------|----------------------|-------------|-------------------------------------|
            |signed with correct key                            |yes               |yes                          |yes    |yes                   |200          |helloWorld                           |1
            |signed with invalid key                            |yes               |yes                          |yes    |no                    |401          |Unauthorized                         |2
            |not signed at all                                  |yes               |yes                          |yes    |no                    |401          |Unauthorized                         |3
            |mac header missing                                 |no                |yes                          |yes    |yes                   |401          |No valid MAC was found in the headers|4
            |access unsecured endpoint using signed request     |yes               |no                           |yes    |yes                   |200          |helloWorld                           |5
            |access unsecured endpoint using unsigned request   |yes               |no                           |no     |yes                   |200          |helloWorld                           |6
            |unsigned accessing signed endpoint                 |na                |yes                          |no     |na                    |401          |No valid MAC was found in the headers|7";

            StoryParser.Parse<ValidMacRequiredIfEndpointIsSecuredRow>(story).ForEach(scenario =>
            {
                MockModule module;
                string privateKey = "4497BB2A-6782-4403-970C-1A7F50BBC7CB";
                string requestPrivateKey = scenario.CorrectKeyUsedToSign ? privateKey : "AAEEDDCC";
                string macSalt = Guid.NewGuid().ToString();
                Trace("");
                TraceTitle(scenario.Title);
                Trace("Given an endpoint " + (scenario.EndpointSecuredWithSimpleMac ? " configured with simple mac" : "with no security"));
                var moduleConfiguration = scenario.EndpointSecuredWithSimpleMac
                    ? _givenAnEchoModuleWithAnEndpointThatIsConfiguredWithSimpleMac(out module, privateKey  )
                    : _givenAnEchoModuleWithAnEndpointThatIsNotConfiguredWithSimpleMac(out module);

                if (!scenario.EndpointSecuredWithSimpleMac)
                {
                    moduleConfiguration.Endpoints.First().Security.Should().Be(SecurityType.none);
                }

                if (scenario.EndpointSecuredWithSimpleMac)
                {
                    //todo: NB! it's not currently obvious (without knowing the code) that OauthRequestAuthenticator requires an instance of ISimpleMacDAL. Change module base to accept the authenticators as dependancies ! That way it will be obvious! doh..bad. ... accepts a lambda that creates the requestAuthenticator... this will eliminate the really bad factory initialisation required below.

                    ObjectFactory.Initialize(x => {
                                x.For<IDateTime>().Use<DateTimeWrapper>();
                                x.For<ISimpleMacDAL>().Use(new SimpleMacDal(new DateTimeWrapper(), new HotwireContext(ConnectionStringManager.HotwireConnectionString)));
                    });
                }

                var requestParameters = new NameValueCollection();
                requestParameters.Add("parameter1", "value1");
                requestParameters.Add("parameter2", "value2");

                MockStreamingContext context = null;

                
                if (scenario.Signed)
                {
                    Trace("And a signed context");
                    // would be excellent if I could create a mock streaming context from a configured httpClient
                    // then the "test" code could be exactly the same for integration tests as it is for unit tests...hmmm?
                    context = new MockStreamingContext(requestParameters, "http://localhost/test/echo/helloWorld", moduleConfiguration);
                    var dateTime = new DateTimeWrapper();
                    var simpleMacSigner = new SimpleMacAuthenticator(dateTime, new SimpleMacDal(new DateTimeWrapper(), new HotwireContext(ConnectionStringManager.HotwireConnectionString)));
                    int timeStamp = dateTime.SecondsSince1970;
                    simpleMacSigner.SignRequestAddToHeaders(context.Headers, requestPrivateKey, requestParameters, context.HttpMethod, context.Url, macSalt, timeStamp);

                }

                if (!scenario.Signed)
                {
                    Trace("And an unsigned context");
                    context = new MockStreamingContext(requestParameters, "http://localhost/test/echo/helloWorld", moduleConfiguration);
                }

                if (!scenario.MacHeaderProvided)
                {
                    context.Headers.Remove(SimpleMACHeaders.HotwireMacHeaderKey);
                }


                Trace("When the module processes the request");
                module.BeginRequest(context);
                context.Should().NotBeNull();

                Trace("Then the response should be :" + scenario.Response);
                context.HttpWriter.ShouldBe(scenario.Response);

                Trace("and the response text should contain '" + scenario.ResponseTextToContain + "'");
                context.MockWriter.ToString().Should().Contain(scenario.ResponseTextToContain);
            });


        }


        public EndpointConfiguration _givenAnEndpoint()
        {
            return new EndpointConfiguration
            {
                Action = MockModule.ActionEcho,
                Active = true,
                HttpMethods = new[] { "GET" },
                MediaType = eMediaType.html,
                Name = "endpoint1",
                UriTemplateString = "/echo/{SAY}"
            };
        }

 
        public EndpointConfiguration _givenASimpleMacEndpoint(string privateKey)
        {
            return new EndpointConfiguration
                       {
                           Action = MockModule.ActionEcho,
                           Active = true,
                           HttpMethods = new[] {"GET"},
                           MediaType = eMediaType.html,
                           Name = "endpoint1",
                           UriTemplateString = "/echo/{SAY}",
                           Security = SecurityType.simpleMAC,
                           PrivateKey = privateKey
                       };
        }


        private ModuleConfigurationDTO _givenAnEchoModuleWithAnEndpointThatIsNotConfiguredWithSimpleMac(out MockModule module)
        {
            Trace("Given an echo module with an endpoint that is not configured with simpleMAC security and a privateKey");
            module = new MockModule();
            var configuration = ModuleConfigurationDTOFactory.GivenModuleConfiguration("test");
            EndpointConfiguration endpoint = _givenAnEndpoint();
            endpoint.Security = SecurityType.none;
            configuration.AddEndpoint(endpoint);
            return configuration;
        }

        private ModuleConfigurationDTO _givenAnEchoModuleWithAnEndpointThatIsConfiguredWithSimpleMac(out MockModule module, string privateKey)
        {
            Trace("Given an echo module with an endpoint that is configured with simpleMAC security and a privateKey");
            module = new MockModule();
            var configuration = ModuleConfigurationDTOFactory.GivenModuleConfiguration("test");
            EndpointConfiguration endpoint = _givenASimpleMacEndpoint(privateKey);
            configuration.AddEndpoint(endpoint);
            return configuration;
        }



    }
}
