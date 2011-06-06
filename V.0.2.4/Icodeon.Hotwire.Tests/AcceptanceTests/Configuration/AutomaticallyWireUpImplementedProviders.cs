using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        [Test]
        public void AllTheFactoryProvidersShouldBeDetectedAndWiredUpWithoutRequiringXmlConfiguration()
        {
            TraceTitle("All the factory providers should be detected and wired up without requiring any xml configuration");

            Trace("Given that the developer (in this case, us the test project) has one and only one class for each of IFileProcessorProvider,IConsumerProvider,IOAuthProvider,IhttpClientProvider");
            Trace("When I ask provider factory for that provider without loading configurations");

            IHttpClientProvider client;
            IFileProcessorProvider fileProcessor;
            IConsumerProvider consumer; 
            IOAuthProvider oauth;
            var factory = new ProviderFactory();

            // fluent assertions here should make this more readable

            Action action;
            action = () => client = factory.CreateHttpClient();
            action.ShouldThrow<AmbiguousMatchException>();

            action = () => fileProcessor = factory.CreateFileProcessor();
            action.ShouldThrow<AmbiguousMatchException>();

            action = () => consumer = factory.CreateConsumerProvider();
            action.ShouldThrow<AmbiguousMatchException>();


            action = () => oauth = factory.CreateOauthProvider();
            action.ShouldThrow<AmbiguousMatchException>();

            Trace("then no instances should be returned");


            Trace("When I Load all the configurations");
            factory.AutoWireUpProviders();

            Trace("and again ask for the providers");
            Trace("HttpClient");
            client = factory.CreateHttpClient();
            Trace("FileProcessor");
            fileProcessor = factory.CreateFileProcessor();
            Trace("Consumer");
            consumer = factory.CreateConsumerProvider();
            Trace("Oauth");
            oauth = factory.CreateOauthProvider();
            
            
            Trace("then valid test providers should be returned.");
            client.Should().NotBeNull();
            client.Should().BeOfType<TestClientProvider>();

            fileProcessor.Should().NotBeNull();
            fileProcessor.Should().BeOfType<TestFileProcessorProvider>();

            consumer.Should().NotBeNull();
            consumer.Should().BeOfType<TestConsumerProvider>();

            oauth.Should().NotBeNull();
            oauth.Should().BeOfType<TestOauthProvider>();
        
        }

        public class TestClientProvider : IHttpClientProvider
        {

            public void GetAndEnsureStatusIsSuccessful(Uri uri)
            {
                throw new NotImplementedException();
            }
            public string GetResponseAsStringEnsureStatusIsSuccessful(Uri uri)
            {
                throw new NotImplementedException();
            }
        }

        public class TestFileProcessorProvider : IFileProcessorProvider
        {
            public void ProcessFile(string resource_file, string transaction_id, System.Collections.Specialized.NameValueCollection requestParams)
            {
                throw new NotImplementedException();
            }
        }

        public class TestConsumerProvider : IConsumerProvider
        {
            public string GetConsumerSecret(string consumerKey)
            {
                throw new NotImplementedException();
            }
        }

        public class TestOauthProvider : IOAuthProvider
        {

            public System.Collections.Specialized.NameValueCollection GenerateSignedParametersForPost(string consumerKey, string consumerSecret, Uri rawUri, System.Collections.Specialized.NameValueCollection nonOAuthParams)
            {
                throw new NotImplementedException();
            }

            public bool IsValidSignatureForPost(string consumerKey, string consumerSecret, Uri uri, System.Collections.Specialized.NameValueCollection form)
            {
                throw new NotImplementedException();
            }
        }
    }
}
