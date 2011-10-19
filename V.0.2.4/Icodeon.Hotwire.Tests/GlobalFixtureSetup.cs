using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using Icodeon.Hotwire.Framework.DAL;
using Icodeon.Hotwire.Framework.Deployment;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Utils;
using NLog;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests
{
    [TestFixture]
    public class GlobalFixtureSetup
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        [TestFixtureSetUp]
        public void SetUp()
        {
            new SchemaChecker(ConnectionStringManager.HotwireConnectionString)
                .CheckSchemaThrowExceptionIfInvalid()
                .ClearoutAnyTestData();
        }

        [Test]
        public void SimpleSchemaCheck()
        {
            // do nothing... can only pass if setup is valid.
        }
    }
}
