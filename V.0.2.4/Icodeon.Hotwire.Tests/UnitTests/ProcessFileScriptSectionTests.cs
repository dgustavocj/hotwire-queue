using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.TestFramework;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.UnitTests
{
    [TestFixture]
    public class ProcessFileScriptSectionTests : UnitTest
    {
        [Test]
        public void CanReadAllValues()
        {
            TraceTitle("CanReadAllValues()");
            var section = ProcessFileScriptSection.ReadConfig();
            section.Endpoint.Should().Be(@"http://localhost:54144/process-file/{TRACKING-NUMBER}.import");
        }


    }
}
