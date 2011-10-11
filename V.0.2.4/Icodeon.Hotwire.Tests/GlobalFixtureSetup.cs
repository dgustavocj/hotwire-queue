using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.DAL;
using Icodeon.Hotwire.Framework.Deployment;
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
            _logger.Debug("");
            _logger.Debug("GlobalFixtureSetup.SetUp()");
            _logger.Debug("--------------------------");
            _logger.Debug("");
            using (var db = new HotwireContext(ConnectionStringManager.HotwireConnectionString))
            {
                if (!db.DatabaseExists()) db.CreateDatabase();
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _logger.Debug("");
            _logger.Debug("GlobalFixtureSetup.TearDown()");
            _logger.Debug("--------------------------");
            _logger.Debug("");
            using (var db = new HotwireContext(ConnectionStringManager.HotwireConnectionString))
            {
                db.DeleteDatabase();
            }
        }


    }
}
