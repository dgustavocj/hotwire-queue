using NLog;
using NUnit.Framework;
using Icodeon.Hotwire.Framework.Diagnostics;

namespace Icodeon.Hotwire.Tests.TestNamespace.SubNamespace
{
    [TestFixture]
    public class SubNestedClass1
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        [Test]
        public void Method1()
        {
            _logger.Trace("Icodeon.Hotwire.Tests.TestNamespace.SubNamespace * SubNestedClass1.Method1()");
        }

        [Test]
        public void Method2()
        {
            _logger.Trace("Icodeon.Hotwire.Tests.TestNamespace.SubNamespace * SubNestedClass1.Method2()");
        }
    }
}

namespace Icodeon.Hotwire.Tests.TestNamespace
{
        [TestFixture]
        public class NestedClass1
        {
            private static Logger _logger = LogManager.GetCurrentClassLogger();
            [Test]
            public void Method1()
            {
                _logger.Trace("Icodeon.Hotwire.Tests.TestNamespace * NestedClass1.Method1() //");
                _logger.Trace("...do something in between");
                _logger.Trace("// Icodeon.Hotwire.Tests.TestNamespace * NestedClass1.Method1()");
            }

            [Test]
            public void Method2()
            {
                _logger.Trace("Icodeon.Hotwire.Tests.TestNamespace * NestedClass1.Method2()");
            }
        }

        [TestFixture]
        public class NestedClass2
        {
            private static Logger _logger = LogManager.GetCurrentClassLogger();
            [Test]
            public void Method1()
            {
                _logger.Trace("Icodeon.Hotwire.Tests.TestNamespace * NestedClass2.Method1()");
            }

            [Test]
            public void Method2()
            {
                _logger.Trace("Icodeon.Hotwire.Tests.TestNamespace * NestedClass2.Method2()");
            }
        }


        [SetUpFixture]
        public class TestNamespaceSetup
        {
            private static Logger _logger = LogManager.GetCurrentClassLogger();

            [SetUp]
            public void Setup()
            {
                _logger.LoggedExecution("Icodeon.Hotwire.Tests.TestNamespace.SubNamespace * TestNamespaceSetup.Setup()", () => _logger.Trace("this line should appear only once for the namespace"));
            }

            [TearDown]
            public void Teardown()
            {
                _logger.LoggedExecution("Icodeon.Hotwire.Tests.TestNamespace.SubNamespace * TestNamespaceSetup.Teardown()", () => _logger.Trace("this line should appear only once for the namespace"));
            }
        }
        
    }