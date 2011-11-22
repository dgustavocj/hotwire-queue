using System;
using Icodeon.Hotwire.Framework.DAL;
using NLog;
using NUnit.Framework;


namespace Icodeon.Hotwire.Tests
{

    [SetUpFixture]
    public class AssemblySetupTearDown
    {
        public AssemblySetupTearDown() {}

        // the attributes are quite similar, and easy to use the wrong one and it looks correct, but is sometimes hellish to debug.
        // here's the actual docs:
        // http://www.nunit.org/index.php?p=setupFixture&r=2.4

        // what's NOT in the docs, is that Resharper will crash and BURN if you place this class outside the namespace in an attempt
        // to get it to run (as per the docs) for the entire assembly! TESTED..FAILS...BADLY, total hang of visual studio.

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        [SetUp]
        public void Init()
        {
            // comment the end of the log with // if the log is mean to have an open and a close log entry. That way we can see if the code exited the method.
            _logger.Debug("AssemblySetupTeardown.Init() //");
            try
            {
                Console.WriteLine("Assembly fixture setup");
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
            finally
            {
                _logger.Debug("// AssemblySetupTeardown.Init() ");
            }
        }

        [TearDown]
        public void Dispose()
        {
            _logger.Debug("AssemblySetupTeardown.Dispose()");
        }

    }

}
