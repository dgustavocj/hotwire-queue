using System;
using System.Net;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Modules;
using Icodeon.Hotwire.Framework.Utils;
using Icodeon.Hotwire.TestFramework;
using Icodeon.Hotwire.Tests.Internal;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.UnitTests
{
    [TestFixture]
    public class EndpointRequestMatcherTests : UnitTest
    {
        //TODO : need a test to show what difference before or after validation makes 
                
        
        private ModuleConfigurationBase _config;

        [SetUp]
        public void Setup()
        {
            _config = new TestModuleConfiguration
            {
                RootServiceName = "test-service",
                Active = true,
                MethodValidation = MethodValidation.beforeUriValidation
            }; 
        }


        [Test]
        public void ShouldReturn404IfExclusiveAndNoMatchingEndpointFound()
        {
            

            TraceTitle("Should return 404 if exclusive is true and no valid match [endpoint] found.");


            Logger.Trace("Given an active configuration with one '/animals/cat.xml' endpoint");
            Logger.Trace("and configuration is Exclusive");
            _config.ExclusiveUse = true;
            _config.MethodValidation = MethodValidation.afterUriValidation;
            var endpoint = new EndpointConfiguration
            {
                Name = "cat",
                Active = true,
                UriTemplate = new UriTemplate("/animals/cat.xml"),
                CommaDelimitedListHttpMethods = "POST"
            };
            _config.AddEndpoint(endpoint);
            var matcher = new EndpointRequestMatcher(_config);
            
            Logger.Trace("WHEN request for '/animals/dog.xml' is made THEN the match should throw HttpModuleException");
            Logger.Trace("AND the httpStatusCode should be 404.");
            EndpointMatch match;
            Action action = () => match = matcher.MatchRequestOrNull("POST", new Uri("http://localhost/test-service/animals/dog.xml"));
            action.ShouldThrow<HttpModuleException>().And.StatusCode.Should().Be(HttpStatusCode.NotFound);

            Logger.Trace("WHEN request for '/animals/cat.xml' is made THEN the match should be found");
            action = () => match = matcher.MatchRequestOrNull("POST", new Uri("http://localhost/test-service/animals/cat.xml"));
            action.ShouldNotThrow();

            Logger.Trace("WHEN exclusive is false AND WHEN a non matching request is made");
            _config.ExclusiveUse = false;
            action = () => match = matcher.MatchRequestOrNull("GET", new Uri("http://localhost/test-service/animals/nosuchanimal.xml"));
            Logger.Trace("THEN the matcher should not throw an exception");
            action.ShouldNotThrow();


        }



        [Test]
        public void ShouldThrowHttpModuleExceptionIfRequestUrlMatchesButHttpMethodNotSupported()
        {
            TraceTitle("Should throw httpModule exception if request url matches but httpMethod not supported");

            // validating the httpMethod "before" other checks is the more performant mechanism, but will return null, instead of throwing an exception.
            // set to validate "after validating the uriTemplate" is the only way for us to accurately tell it was a template match, without a matching httpMethod.
            _config.MethodValidation = MethodValidation.afterUriValidation;

            Logger.Trace("Given an active configuration with one '/animals/cat.xml' endpoint only allowing POST verb");

            var endpoint = new EndpointConfiguration
                               {
                                   Name = "cat",
                                   Active = true,
                                   UriTemplate = new UriTemplate("/animals/cat.xml"),
                                   CommaDelimitedListHttpMethods = "POST"
                               };
            _config.AddEndpoint(endpoint);
            var matcher = new EndpointRequestMatcher(_config);

            Logger.Trace("WHEN a POST request for '/animals/cat.xml' is made THEN the match is successful");
            EndpointMatch match = matcher.MatchRequestOrNull("POST", new Uri("http://localhost/test-service/animals/cat.xml"));
            match.Should().NotBeNull();
            match.Endpoint.Name.Should().Be("cat");
            
            Logger.Trace("WHEN a GET request for '/animals/cat.xml' is made THEN the match should throw HttpModuleException");
            Logger.Trace("AND the httpStatusCode should be MethodNotAllowed.");
            Action action = ()=> match = matcher.MatchRequestOrNull("GET", new Uri("http://localhost/test-service/animals/cat.xml"));
            action.ShouldThrow<HttpModuleException>()
                .And
            .StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
        }

        [Test]
        public void LeadingSlashInEndpointConfigurationIsOptional()
        {
            TraceTitle("Leading slash in endpoint configuration is optional.");

            Logger.Trace("Given cat endpoint of '/animals/cat.xml' and dog endpoint of 'animals/dog.xml'");
            //---------------------------------------------------------------------------------------------
            var catEndpoint = new EndpointConfiguration {
                Name = "cat",
                Active = true,
                UriTemplate = new UriTemplate("/animals/cat.xml"),
                CommaDelimitedListHttpMethods = "POST"
            };
            var dogEndpoint = new EndpointConfiguration {
                Name = "dog",
                Active = true,
                UriTemplate = new UriTemplate("animals/dog.xml"),
                CommaDelimitedListHttpMethods = "POST"
            };
            _config.AddEndpoint(catEndpoint);
            _config.AddEndpoint(dogEndpoint);
            var matcher = new EndpointRequestMatcher(_config);

            Logger.Trace("THEN POST requests for the cat or dog resource should both be successful");
            //---------------------------------------------------------------------------------------
            var match = matcher.MatchRequestOrNull("POST", new Uri("http://localhost/test-service/animals/cat.xml"));
            match.Should().NotBeNull();
            match.Endpoint.Name.Should().Be("cat");
            match = matcher.MatchRequestOrNull("POST", new Uri("http://localhost/test-service/animals/dog.xml"));
            match.Should().NotBeNull();
            match.Endpoint.Name.Should().Be("dog");
        }


// the check to ensure you cannot create a matcher with a null configuration is only applied during debug builds.
// the reason for this, is that endpoint request matcher is called for every single http request that the website
// will recieve and it uses reflection.
#if DEBUG 
        [Test]
        public void MustNotBePossibleToCreateMatcherWithANullConfiguration()
        {
            EndpointRequestMatcher matcher;
            Action action = ()=> matcher = new EndpointRequestMatcher(null);
            action.ShouldThrow<ArgumentNullException>();
        }
#endif

        [Test]
        public void InactiveConfigurationsAreIgnored()
        {
            TraceTitle("Inactive configurations are ignored");

            Logger.Trace("Given an active module configuration with a single endpoint");
            var endpoint = new EndpointConfiguration
                               {
                                   Action = "test",
                                   Name = "test-endpoint",
                                   Active = true,
                                   UriTemplateString = "/cars/bmw.xml",
                                   CommaDelimitedListHttpMethods = "GET"
                               };
            _config.AddEndpoint(endpoint);
            _config.Active = true;

            Trace("when I check for a request Match");
            var matcher = new EndpointRequestMatcher(_config);
            var result = matcher.MatchRequestOrNull("GET", new Uri("http://localhost/test-service/cars/bmw.xml"));

            Trace("then the match is returned.");
            result.Should().NotBeNull();
            result.Endpoint.Name.Should().Be("test-endpoint");

            Trace("Given an INACTIVE configuration");
            _config.Active = false;

            Trace("when I check for a request Match");
            result = matcher.MatchRequestOrNull("GET", new  Uri("http://localhost/test-service/cars/bmw.xml"));

            Trace("then the match is ignored.");
            result.Should().BeNull();
        }

        [Test]
        public void EndpointsCanBeMarkedActiveOrInactive()
        {
            TraceTitle("Endpoints can be marked Active or Inactive");

            Logger.Trace("Given two endpoint configurations, /animals/cat.xml (active) and /animals/dog.xml (inactive)");
            var catEndpoint = new EndpointConfiguration
            {
                Name = "cat",
                Active = true,
                UriTemplate = new UriTemplate("/animals/cat.xml"),
                CommaDelimitedListHttpMethods = "POST"
            };
            var dogEndpoint = new EndpointConfiguration
            {
                Name = "dog",
                Active = false,
                UriTemplate = new UriTemplate("/animals/dog.xml"),
                CommaDelimitedListHttpMethods = "POST"
            };
            _config.AddEndpoint(catEndpoint);
            _config.AddEndpoint(dogEndpoint);
            var matcher = new EndpointRequestMatcher(_config);

            Trace("When I request a match for cat");
            var match = matcher.MatchRequestOrNull("POST", new Uri("http://localhost/test-service/animals/cat.xml"));

            Trace("Then the cat match should be succesful");
            match.Should().NotBeNull();
            match.Endpoint.Name.Should().Be("cat");

            Trace("When I request a match for dog");
            match = matcher.MatchRequestOrNull("POST", new Uri("http://localhost/test-service/animals/dog.xml"));

            Trace("Then the dog match should not be succesful");
            match.Should().BeNull();

            Trace("When I enable the dog endpoint and request a match for dog again");
            dogEndpoint.Active = true;
            match = matcher.MatchRequestOrNull("POST", new Uri("http://localhost/test-service/animals/dog.xml"));

            Trace("Then the dog match should be succesful");
            match.Should().NotBeNull();
            match.Endpoint.Name.Should().Be("dog");
            
        }



    }
}
