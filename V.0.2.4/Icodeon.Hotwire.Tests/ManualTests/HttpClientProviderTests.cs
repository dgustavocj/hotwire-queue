using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Http;
using Icodeon.Hotwire.Tests.Internal;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.UnitTests
{
    [TestFixture]
    public class HttpClientProviderTests
    {
        // this test will not pass on the/a build server, until I can merge existing cassini runner code that starts the webserver,
        // or, update build script to deploy the test website as part of the build.
        // for now, this test will have to run from within visual studio, with the "always start when debugging=true" project setting on TestAspNet
        [Test]
        public void AsyncWrappingShouldWaitForGetToCompleteBeforeReturning()
        {
            var client = new HttpClientProvider();
            Action shouldPass = ()=> client.GetAndEnsureStatusIsSuccessful(new Uri(TestAspNet.RootUri + TestAspNet.MockCDN + "Test.txt" ));
            shouldPass.ShouldNotThrow();
            Action shouldNotPass = () => client.GetAndEnsureStatusIsSuccessful(new Uri(TestAspNet.RootUri + TestAspNet.MockCDN + "NotExist.txt"));
            shouldNotPass.ShouldThrow<Exception>();
        }

        [Test]
        public void AsyncWrappingShouldWaitForGetToCompleteBeforeReturningAndReturnText()
        {
            var client = new HttpClientProvider();
            string response =null;
            Action shouldPass = () => response = client.GetResponseAsStringEnsureStatusIsSuccessful(new Uri(TestAspNet.RootUri + TestAspNet.MockCDN + "Test.txt"));
            shouldPass.ShouldNotThrow();
            response.Should().NotBeNull();
            response.Should().Contain("Hello world 1234");
            
            response = null;
            string url = TestAspNet.RootUri + "Throws.aspx";
            Action shouldNotPass = () => response = client.GetResponseAsStringEnsureStatusIsSuccessful(new Uri(url));
            shouldNotPass
                .ShouldThrow<Exception>()
                .And.Message.Should().Contain("500");


            Action notExist = () => response = client.GetResponseAsStringEnsureStatusIsSuccessful(new Uri(TestAspNet.RootUri + TestAspNet.MockCDN + "ThisDoesNotExist.txt"));
            notExist
                .ShouldThrow<Exception>()
                .And.Message.Should().Contain("404");

        }

    }
}
