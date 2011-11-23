using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.FolderWatcher;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Utils;
using Icodeon.Hotwire.TestFramework;
using Icodeon.Hotwire.TestFramework.Mocks;
using Icodeon.Hotwire.Tests.Internal;
using NLog;
using NUnit.Framework;
using Line = Icodeon.Hotwire.TestFramework.Mocks.MockConsole.Line;

namespace Icodeon.Hotwire.Tests.AcceptanceTests.FolderWatcher
{
    [TestFixture]
    public class AutostartMonitoringTests : FilesProviderUnitTest
    {

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        [SetUp]
        public void Setup()
        {
            FilesProvider.EmptyAllFolders();
        }


        [TearDown]
        public void TearDown()
        {
            FilesProvider.EmptyAllFolders();
        }


        [Test]
        public void ShouldDownloadAndThenProcessEnqueuedRequests()
        {
            TraceTitle("ShouldDownloadAndThenProcessEnqueuedRequests() - Should download and then process an enqueued request");
            Trace("Given a folderwatcher program");
            Trace("And a mock console that simulates a user who types 'download' then 'exit' at the console after the files have been processed");
            Trace("And I start both download and process monitoring");

            // logging below is for debugging logfiles, tracing above is for team city and resharper build output
            // will need to think how to avoid the duplication. Would be great if "Debug" level was simply the readable, "team city" output, info was methods, and trace was, well, "trace" i.e. detailed.

            _logger.Trace("");
            _logger.Trace("ShouldDownloadAndThenProcessEnqueuedRequests()");

            var testData = new TestData(FilesProvider);

            Action createImportWaitForItToBeProcessed = () =>
            {
                testData.CreateTestEnqueueRequestImportFile(Guid.NewGuid(), "Testfile.txt",30); 
                testData.CreateTestEnqueueRequestImportFile(Guid.NewGuid(), "hello.txt",30); 
                testData.CreateTestEnqueueRequestImportFile(Guid.NewGuid(), "Testfile.txt",30); 
                int cnt = 0;
                int cntFiles;
                while ((cntFiles = FilesProvider.ProcessedFilePaths.Count()) != 3)
                {
                    _logger.Trace("ProcessedFilePaths.Count():{0}", cntFiles);
                    Thread.Sleep(100);
                    if (cnt++ > 20) throw new Exception("timeout waiting 6 seconds for all the files to be downloaded and processed.");
                    FilesProvider.RefreshFiles();
                }
            };

            var mockconsole = new MockConsole(
                new List<Line> { new Line(null, "start"), new Line(createImportWaitForItToBeProcessed, "exit") },
                new DateTimeWrapper(),
                true);

            var mockProcessor = new MockProcessFileCaller(null);
            var mockDownloder = new MockDownloader(null);
            
            FilesProvider.DownloadQueueFilePaths.Count().Should().Be(0);
            FilesProvider.ProcessedFilePaths.Count().Should().Be(0);
            Trace("");
            Trace("----------------captured console output-----------------------------");
            FolderWatcherCommandProcessor.RunCommandProcessorUntilExit(false, FilesProvider, mockconsole, new DateTimeWrapper(), mockProcessor,mockDownloder);
            Trace("----------------/captured console output----------------------------");
            FilesProvider.RefreshFiles();

            Trace("Then the the files should be downloaded");
            FilesProvider.DownloadQueueFilePaths.Count().Should().Be(0);
            FilesProvider.DownloadErrorFilePaths.Count().Should().Be(0);

            Trace("And the files should be processed");
            FilesProvider.ProcessQueueFilePaths.Count().Should().Be(0);
            FilesProvider.ProcessErrorFilePaths.Count().Should().Be(0);
            FilesProvider.ProcessingFilePaths.Count().Should().Be(0);
            FilesProvider.ProcessedFilePaths.Count().Should().Be(3);
        }

    }
}
