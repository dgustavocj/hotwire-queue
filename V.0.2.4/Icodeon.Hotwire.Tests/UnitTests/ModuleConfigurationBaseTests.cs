using System.Linq;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.MediaTypes;
using Icodeon.Hotwire.TestFramework;
using Icodeon.Hotwire.Tests.Internal;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.UnitTests
{
    [TestFixture]
    public class ModuleConfigurationBaseTests : UnitTest
    {
        [Test]
        public void CanReadModuleAndEndpointConfiguration()
        {
            TraceTitle("Can read Module and Endpoint configuration:");
            Trace(
                @"Given appropriate custom config section and custom config entry has been added to test projects's App.Config, similar to the following:
                 ----------------------------------------------------------------------------------------------------------------------------------------
                <configSections>
                    <sectionGroup name='hotwire'>
                      <section name='test-module-config' type='Icodeon.Hotwire.Tests.Framework.TestModuleConfiguration, Icodeon.Hotwire.Tests' />
                    </sectionGroup>
                </configSections>
                ...
                and
                ...
              <hotwire>
                <test-module-config active='true' rootServiceName='test-animals' methodValidation='afterUriValidation'>
                  <endpoints>
                    <add name='cat' active='true' uriTemplate='/cat.xml' action='action-cat' httpMethods='GET,POST' mediaType='xml' security='none' />
                    <add name='dog' active='false' uriTemplate='/dog.xml' action='action-dog' httpMethods='GET' mediaType='json' security='none' />
                  </endpoints>
                </test-module-config>
              </hotwire>
");

            // wording used above is "similar" so that we don't have to keep refactoring this trace log each time a small change is made.

            Trace("When the test configuration is read");
            IModuleConfiguration config = new TestModuleConfiguration().ReadConfig();

            Trace("Then the configuration should be read correctly.");
            EnsureTestModuleConfigurationAndEndpointsAreReadCorrectly(config);

        }

        public static void EnsureTestModuleConfigurationAndEndpointsAreReadCorrectly(IModuleConfiguration config)
        {
            config.Ensure(c => c.Active,
                          c => c.Debug,
                          c => c.RootServiceName == "test-animals",
                          c => c.MethodValidation == MethodValidation.afterUriValidation,
                          c => c.Endpoints != null && c.Endpoints.Count() == 2);

            var catEndpoint = config.Endpoints.ElementAt(0);

            catEndpoint.Ensure(c => c.Active,
                                c => c.Name == "cat",
                                c => c.UriTemplate.ToString() == "/cat.xml",
                                c => c.Action == "action-cat",
                                c => c.Security == SecurityType.none,
                                c => c.MediaType == eMediaType.xml);

            catEndpoint.HttpMethods.Should().Equal(new[] { "GET", "POST" });

            var dogEndpoint = config.Endpoints.ElementAt(1);

            dogEndpoint.Ensure(c => c.Active,
                                c => c.Name == "dog",
                                c => c.UriTemplate.ToString() == "/dog.xml",
                                c => c.Action == "action-dog",
                                c => c.Security == SecurityType.none,
                                c => c.MediaType == eMediaType.json);

            dogEndpoint.HttpMethods.Should().Equal(new[] { "GET" });
            
        }


    }
}
