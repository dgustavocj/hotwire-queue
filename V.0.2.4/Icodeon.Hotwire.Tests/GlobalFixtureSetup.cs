using System;
using Icodeon.Hotwire.Framework.DAL;
using NLog;
using NUnit.Framework;

    [SetUpFixture]
    public class AssemblySetupTeardown
    {
        // the attributes are quite similar, and easy to use the wrong one and it looks correct, but is sometimes hellish to debug.
        // here's the actual docs:
        // http://www.nunit.org/index.php?p=setupFixture&r=2.4

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        // not once per namespace, doh! it's once ...per this group of tests...grrr!
        [SetUp]
        public void Init()
        {
            _logger.Debug("GlobalFixtureSetup.Init()");
            try
            {

                var checker = new SchemaChecker(ConnectionStringManager.HotwireConnectionString);
                checker.CheckSchemaThrowExceptionIfInvalid();
                checker.ClearoutAnyTestData();
            }
            catch (Exception ex)
            {
                _logger.Debug("Exception during GlobalFixtureSetup()");
                _logger.Debug(ex.Message);
                throw;
            }
        }

        [TearDown]
        public void Dispose()
        {
            _logger.Debug("GlobalFixtureSetup.Dispose()");
        }

        [Test]
        public void SimpleSchemaCheck()
        {
            // do nothing... can only pass if setup is valid.
        }
    }
