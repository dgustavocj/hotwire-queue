using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.TestFramework;
using NUnit.Framework;
using StructureMap;

namespace Icodeon.Hotwire.Tests.AcceptanceTests.Configuration
{
    [TestFixture]
    public class AutomaticallyWireUpImplementedProviders : UnitTest
    {
        //ADH: 31.05.2011 NB! Will need to remember to manually exclude the test assembly by name from any auto wiring up!
        //                    otherwise any test classes could be wired up by mistake instead of the developers implementation.
        //                    don't want any abiguities.

        // Need to see if this test passes when run twice in a row? i.e. running structuremap /IOC "stuff" from within unit tests
        // may require that we call some sort of structureMap reset? requires some reading. This could cause side effects in other unit tests
        // if not careful.

        [Test]
        public void HttpClientProviderShouldBeDetected()
        {
            TraceTitle("Http client provider should be detected and wired up without requiring any xml configuration");

            Trace("Given that the developer has implemented one and only one class that implements a specific provider");
            Trace("When I ask provider factory for that provider");
            IHttpClientProvider client;
            try
            {
                client = new ProviderFactory(HotLogger.NullLogger).CreateHttpClient();
                Assert.Fail("Structuremap should not have returned an instance as mapping has not be configured!");
            }
            catch (StructureMapException sme)
            {
                Trace("then an instance should not be returned");
            }


            Trace("When I Load all the configurations");
            new Configurator().AutoWireUpProviders();

            Trace("and again ask for a provider");
            client = new ProviderFactory(HotLogger.NullLogger).CreateHttpClient();

            Trace("then a valid test provider should be returned.");
            client.Should().NotBeNull();
            client.Should().BeAssignableTo<IHttpClientProvider>();
            string result = client.GetResponseAsStringEnsureStatusIsSuccessful(null);
            result.Should().Be("I am a TestClientProvider");
        }

        public class TestClientProvider : IHttpClientProvider
        {

            public void GetAndEnsureStatusIsSuccessful(Uri uri)
            {
                throw new NotImplementedException();
            }
            public string GetResponseAsStringEnsureStatusIsSuccessful(Uri uri)
            {
                return "I am a TestClientProvider";
            }
        }
    }
}
