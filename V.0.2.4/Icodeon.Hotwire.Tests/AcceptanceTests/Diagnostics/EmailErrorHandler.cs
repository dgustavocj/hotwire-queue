using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.AcceptanceTests.Diagnostics
{
    [TestFixture("acceptance-test", "As a service provider, so that I dont have to keep watching the server, I want to be able to receive error notifications by email.")]
    public class EmailErrorHandler
    {
        [Test]
        public void EmailToIncludeAllTheExceptionDetails()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void ShouldBeAbleToSendEmail()
        {
            throw new NotImplementedException();
        }

    }
}
