using System.Linq;
using System.Text;
using System.Web;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Utils;
using Icodeon.Hotwire.TestFramework;
using Icodeon.Hotwire.TestFramework.Mocks;
using Icodeon.Hotwire.Tests.Internal;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.UnitTests
{
    [TestFixture]
    public class ModuleBaseTests : UnitTest
    {
        [Test]
        public void ShouldBeTestableWithoutRequiringExternalDependancies()
        {
            TraceTitle("ShouldBeTestableWithoutRequiringExternalDependancies() - Should be testable without requiring external website or other dependancies");

            Trace("Given a module that will respond with a quote of the day");
            var module = new MockModule();

            Trace("and a mock session context");
            var context = new MockParsedContext(MockModule.ActionRun);

            Trace("When I call process");
            var result = module.ProcessRequest(context);

            Trace("then I should get the QOTD");
            result.Should().Be("Never run with scissors");
        }
    }
}
