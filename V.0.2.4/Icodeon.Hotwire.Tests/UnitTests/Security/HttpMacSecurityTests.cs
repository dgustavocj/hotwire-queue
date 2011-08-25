using System;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.UnitTests.Security
{
    [TestFixture]
    public class HttpMacSecurityTests
    {
        // see w3 sec 14.8 Authorization 
        //http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html
        // not implementing "authorization" RFC here, since we're not providing access to network resources, but a single "once off" validation of an API call.

        [Test]
        public void ShouldPlaceMacAndSaltInTheHeaders()
        {
            throw new Exception("not implemented");

            //TraceTitle("Should validate Mac signed requests");
            //Trace("Given random string of sds23dfsdf23");
            //Trace("and paramters of -> 12, 'cat', 123.1121212");
        }

        [Test]
        public void ShouldAuthenticateSignedRequest()
        {
            throw new Exception("not implemented");
        }

        [Test]
        public void ShouldNotAuthenticateUnsignedRequest()
        {
            throw new Exception("not implemented");
        }

        [Test]
        public void ShouldEnsureSaltIsAGuid()
        {
            throw new Exception("not implemented");
        }

        [Test]
        public void ShouldReturn400IfSaltMissingOrNotAGuid()
        {
            throw new Exception("not implemented");
        }

        [Test]
        public void IfSaltIsMissingResponseShouldContainHumanReadableExplanationThatSaltMustBeAGuid()
        {
            throw new Exception("not implemented");
        }

    }
}
