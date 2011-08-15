using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Diagnostics;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.UnitTests
{
    [TestFixture]
    public class SSLEmailErrorHandlerConfigurationTests
    {
        [Test]
        public void CanReadAllValues()
        {
            var configuration = new SSLEmailErrorHandlerConfiguration().ReadConfig();
            
            // can't mix and match simple Ensure's together with fluent's Should.Equal's Constraint return type
            // so have to be on seperate lines here.

            configuration.ToAddresses.Should().Equal(new[] {"toTest1@test.com", "toTest2@test.com"});

            configuration.Ensure(c => c != null,
                                 c => c.ConfigurationSectionName == "sslEmailErrorHandler",
                                 c => c.FromAddress == "test@test.com",
                                 c => c.TimeoutSeconds == 5,
                                    c => c.SubjectLinePrefix == "xxx subject line prefix"
                                    );

            // need to include configuration of what it handles!

        }
    }
}
