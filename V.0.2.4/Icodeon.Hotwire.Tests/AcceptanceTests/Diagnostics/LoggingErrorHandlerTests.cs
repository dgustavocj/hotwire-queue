using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Utils;
using Icodeon.Hotwire.TestFramework;
using Icodeon.Hotwire.TestFramework.Mocks;
using NLog;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.AcceptanceTests.Diagnostics
{
    [TestFixture(Category = "acceptanceTest")]
    public class LoggingErrorHandlerTests : UnitTest
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private FileInfo _logfile;

        [SetUp]
        public void Setup()
        {
            _logger.LoggedExecution("LoggingErrorHandlerTests.Setup()" , ()=>{
                _logfile = DirectoryHelper.GetCurrentDirectoryLogfile(NLogConfigConstants.ErrorHandler);
                _logger.Trace("_logfile:{0}", _logfile);
                if (_logfile.Exists) _logfile.Delete();
            });
        }

        [TearDown]
        public void Teardown()
        {
            _logger.Trace("LoggingErrorHandlerTests.Teardow()");
            try
            {
                _logfile.Delete();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
                throw;
            }

        }

        [Test]
        public void ShouldCreateLogEntryWhenAnExceptionOccurs()
        {
            TraceTitle("ShouldCreateLogEntryWhenAnExceptionOccurs() - should create a log entry when an exception occurs");

            Trace("Given a module that will respond with an httpException");
            var module = new MockModule();
            var configuration = ModuleConfigurationDTOFactory.GivenModuleConfigurationForMockModule();
            var streamingContext = new MockStreamingContext("http://localhost/throw/httpexception", configuration);

            Trace("And a LoggingError handler to handle the exceptions");
            var errorHandler = new LoggingErrorHandler();
            module.ProcessRequestException += errorHandler.HandleException;

            Trace("When I call BeginRequest");
            module.BeginRequest(streamingContext);

            TraceFooter("then an entry should be created in the log file.");
            ThenAnEntryShouldBeCreatedInTheLog("HttpModuleException");
        }

        private void ThenAnEntryShouldBeCreatedInTheLog(string entry)
        {
            if (!File.ReadAllText(_logfile.FullName).Contains(entry)) 
                Assert.Fail("Could not find '{0}' in logfile :{1}.",entry, _logfile.Name);
        }
    }
}
