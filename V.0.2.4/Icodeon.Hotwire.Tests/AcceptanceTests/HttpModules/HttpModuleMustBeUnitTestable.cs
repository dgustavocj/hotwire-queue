using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.AcceptanceTests.HttpModules
{
    [TestFixture]
    public class HttpModuleMustBeUnitTestable //: UnitTest
    {
        [Test]
        public void EndpointConfigurationMustBeUnitTestable()
        {
            Assert.Inconclusive();
            // given an httpModule
            // and an endpoint configuration
            // when I trigger the endpoint
            // then the configured action for the matching endpoint is passed to the module as a parameter
        }
    }
}
