using System.Linq;
using FluentAssertions;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Contracts.Enums;
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
            TraceTitle("CanReadModuleAndEndpointConfiguration() - Can read Module and Endpoint configuration:");
            Trace("Given appropriate custom config section and custom config entry has been added to test projects's App.Config");

            Trace("When the test configuration is read");
            IModuleConfiguration config = new TestModuleConfiguration().ReadConfig<IModuleConfiguration>();

            Trace("Then the configuration should be read correctly.");
            EnsureTestModuleConfigurationAndEndpointsAreReadCorrectly(config);
            var secureEndpoint = config.Endpoints.FirstOrDefault(ep => !string.IsNullOrEmpty(ep.PrivateKey));
            secureEndpoint.Should().NotBeNull();
            secureEndpoint.PrivateKey.Should().Be("my private key 1234");
        }

        //TODO: see if I can dynamically return different types of endpoints (e.g. simpleMacEndpoint ... instead of using nulls for all other non secured endpoints) ?

        public static void EnsureTestModuleConfigurationAndEndpointsAreReadCorrectly(IModuleConfiguration config)
        {
            config.Ensure(c => c.Active,
                          c => c.Debug,
                          c => c.RootServiceName == "test-animals",
                          c => c.MethodValidation == MethodValidation.afterUriValidation,
                          c => c.Endpoints != null && c.Endpoints.Count() == 3);

            var catEndpoint = config.Endpoints.ElementAt(0);

            catEndpoint.Ensure(c => c.Active,
                                c => c.Name == "cat",
                                c => c.PrivateKey == "",
                                c => c.TimeStampMaxAgeSeconds == null,
                                c => c.UriTemplate.ToString() == "/cat.xml",
                                c => c.Action == "action-cat",
                                c => c.Security == SecurityType.none,
                                c => c.MediaType == eMediaType.xml);

            catEndpoint.HttpMethods.Should().Equal(new[] { "GET", "POST" });

            var dogEndpoint = config.Endpoints.ElementAt(1);

            dogEndpoint.Ensure(c => c.Active,
                                c => c.Name == "dog",
                                c => c.PrivateKey == "",
                                c => c.TimeStampMaxAgeSeconds == null,
                                c => c.UriTemplate.ToString() == "/dog.xml",
                                c => c.Action == "action-dog",
                                c => c.Security == SecurityType.none,
                                c => c.MediaType == eMediaType.json);

            dogEndpoint.HttpMethods.Should().Equal(new[] { "GET" });

            var securedog = config.Endpoints.ElementAt(2);

            securedog.Ensure(c => c.Active,
                                c => c.Name == "securedog",
                                c => c.PrivateKey == "my private key 1234",
                                c => c.TimeStampMaxAgeSeconds == 10,
                                c => c.UriTemplate.ToString() == "/secure/dog.xml",
                                c => c.Action == "action-dog",
                                c => c.Security == SecurityType.simpleMAC,
                                c => c.MediaType == eMediaType.json);

            securedog.HttpMethods.Should().ContainInOrder(new[] {"PUT", "GET", "POST"});

        }


    }
}
