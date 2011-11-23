using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
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
    public class DownloadQueueMonitoringTests : FilesProviderUnitTest
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
        public void ShouldBeAbleToHandleAfloodOfImportFilesWithoutMissingAny()
        {
            TraceTitle("Should be able to handle a flood of import files (50) without missing any");
            Trace("Given a folderwatcher program");
            Trace("And an empty process queue");
            FilesProvider.ProcessQueueFilePaths.Count().Should().Be(0);
            
            Trace("And a mock console that simulates a user who types 'download' then 'exit' at the console");

            var testData = new TestData(FilesProvider);
            Action createImportWaitForItToBeProcessed = () => {
                for (int i = 1; i < 26; i++) {
                    testData.CreateTestEnqueueRequestImportFile(Guid.NewGuid(), "Testfile" + i + ".txt",null);
                    testData.CreateTestEnqueueRequestImportFile(Guid.NewGuid(), "hello" + i  + ".txt",null);
                }
                int cnt = 0;
                int numFiles;
                while ((numFiles = FilesProvider.ProcessQueueFilePaths.Count()) != 50) {
                    Thread.Sleep(500);
                    if (cnt++ > 8)
                    {
                        var msg = "timeout waiting 4 seconds for all the files to be downloaded. numFiles=" + numFiles;
                        _logger.Fatal(msg);
                        throw new Exception(msg);
                    }
                    FilesProvider.RefreshFiles(); 
                }
            };

            var mockconsole = new MockConsole(
                new List<Line> { new Line(null, "download"), new Line(createImportWaitForItToBeProcessed, "exit") }, 
                new DateTimeWrapper(), 
                true);

            var mockProcessor = new MockProcessFileCaller(null);
            // NB! we are using fake files that do not exist in the CDN, so a real downloader will not work below, only a mock will work.
            var mockDownloader = new MockDownloader(null);

            Trace("When I create a 'flood' of enqueue requests (50 import files)");
            Trace("And I start download monitoring");

            Trace("");
            Trace("----------------captured console output-----------------------------");
            FolderWatcherCommandProcessor.RunCommandProcessorUntilExit(false, FilesProvider, mockconsole, new DateTimeWrapper(),mockProcessor, mockDownloader);
            Trace("----------------/captured console output----------------------------");
            FilesProvider.RefreshFiles();

            Trace("Then the FolderWatcher should start the downloader and download all the files");
            Trace("Then there should be 0 files in the download queue");
            FilesProvider.DownloadQueueFilePaths.Count().Should().Be(0);
            Trace("and there should be 50 files in the process queue");
            FilesProvider.ProcessQueueFilePaths.Count().Should().Be(50);
        }

        private static readonly Action<WaitTillComplete> WaitTillFilesAreDownloaded = (wtc) => {
            int cnt = 0;
            int numFiles;
            int cntMax = (wtc.SecondsToWait*1000) / 500;
            while ((numFiles = wtc.FilesProvider.ProcessQueueFilePaths.Count()) !=  wtc.NumFiles)
            {
                _logger.Trace("\tProcessQueueFilePaths:{0}", numFiles);
                Thread.Sleep(500);
                if (cnt++ > cntMax) throw new Exception("timeout waiting " + wtc.SecondsToWait + " seconds for all the files to be downloaded. Current count is " + numFiles);
                wtc.FilesProvider.RefreshFiles();
            }
            _logger.Debug("all good! There are/is now {0} file/s in the ProcessQueue.", numFiles);
            _logger.Debug("");
        };

        private class WaitTillComplete
        {
            public int SecondsToWait { get; set; }
            public int NumFiles { get; set; }
            public HotwireFilesProvider FilesProvider { get; set; }
        }

        [Test]
        public void NewEnqueueRequestInDownloadQueueShouldTriggerFileDownloadScript()
        {
            TraceTitle("New enqueue request in download queue should trigger file download script");
            Trace("Given a folderwatcher program");
            Trace("And a mock console that simulates a user who types 'download' then 'exit' at the console");
            Trace("When I create an enqueue requests in the download queue folder");
            Trace("And I start download monitoring");

            _logger.Trace("");
            _logger.Trace("NewEnqueueRequestInDownloadQueueShouldTriggerFileDownloadScript()");

            var testData = new TestData(FilesProvider);

            Action createImportWaitForItToBeProcessed = () => {
                testData.CreateTestEnqueueRequestImportFile(Guid.NewGuid(), "Testfile.txt",null);
                WaitTillFilesAreDownloaded(new WaitTillComplete { FilesProvider = FilesProvider, NumFiles = 1,SecondsToWait = 4 });
            };

            var mockconsole = new MockConsole(
                new List<Line> { new Line(null, "download"), new Line(createImportWaitForItToBeProcessed, "exit") },
                new DateTimeWrapper(),
                true);

            var mockProcessor = new MockProcessFileCaller(null);

            var mockDownloader = new MockDownloader(null);
            
            // don't enable a real downloader (below) if you don't have Icodeon.Hotwire.TestAspNet running
            //x var mockDownloader = new HotClient();
            // otherwise you will get 404 errors when it tries to download left in here 
            // for diagnostics. In some cases it's useful to do a real end to end.

            Trace("");
            Trace("----------------captured console output-----------------------------");
            FolderWatcherCommandProcessor.RunCommandProcessorUntilExit(false, FilesProvider, mockconsole, new DateTimeWrapper(), mockProcessor, mockDownloader);
            Trace("----------------/captured console output----------------------------");
            FilesProvider.RefreshFiles();

            Trace("Then the Download script should be started");
            Trace("And the file should be downloaded and saved in the process queue folder");
            FilesProvider.DownloadQueueFilePaths.Count().Should().Be(0);
            FilesProvider.ProcessQueueFilePaths.Count().Should().Be(1);
        }

    }
}
