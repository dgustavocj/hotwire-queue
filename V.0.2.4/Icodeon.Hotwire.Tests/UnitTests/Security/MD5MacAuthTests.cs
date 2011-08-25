using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.TestFramework;
using NUnit.Framework;

namespace Icodeon.OUIntegration.Tests.AcceptanceTests.Security
{
    [TestFixture]
    public class MD5MacAuthTests : UnitTest
    {


        [Test]
        public void ShouldThrowExceptionIfSaltIsNotAGuid()
        {
            TraceTitle("should generate hash");
            Trace("Given ");
        }


        [Test]
        public void ShouldGenerateHash()
        {
            TraceTitle("should generate hash");
            Trace("Given ");

        }

        [Test]
        public void ShouldValidateMacSignedRequests()
        {
            throw new Exception("not implemented");

        }



    }
}
