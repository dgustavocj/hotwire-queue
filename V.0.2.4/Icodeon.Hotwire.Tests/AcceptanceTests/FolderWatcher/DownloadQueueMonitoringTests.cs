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

        }


        [TearDown]
        public void TearDown()
        {
            FilesProvider.EmptyAllFolders();
        }


        // TODO: this test ... as it stands will run on a local dev machine as long as aspNet is running, and will fail on the build server
        // TODO: unless I have some means of ensuring that the website is running at the time of the test, i.e. outside of visual studio.
        // TODO: need to port the test runner... take a look at various open source projects for inspiration! 

        [Test]
        public void ShouldBeAbleToHandleAfloodOfImportFilesWithoutMissingAny()
        {
            TraceTitle("Should be able to handle a flood of import files (50) without missing any");
            Trace("Given a folderwatcher program");
            Trace("And a mock console that simulates a user who types 'download' then 'exit' at the console");
            Trace("When I create a 'flood' of enqueue requests (50 import files)");
            Trace("And I start download monitoring");
            
            var testData = new TestData(FilesProvider);

            Action createImportWaitForItToBeProcessed = () => {
                for (int i = 0; i < 25; i++) {
                    testData.CreateTestEnqueueRequestImportFile(Guid.NewGuid(), "Testfile.txt");
                    testData.CreateTestEnqueueRequestImportFile(Guid.NewGuid(), "hello.txt");
                }
                int cnt = 0;
                while (FilesProvider.ProcessQueueFilePaths.Count() != 50) {
                    Thread.Sleep(500);
                    if (cnt++ > 8) throw new Exception("timeout waiting 4 seconds for all the files to be downloaded.");
                    FilesProvider.RefreshFiles(); 
                }
            };

            var mockconsole = new MockConsole(
                new List<Line> { new Line(null, "download"), new Line(createImportWaitForItToBeProcessed, "exit") }, 
                new DateTimeWrapper(), 
                true);

            var mockProcessor = new MockProcessFileCaller(null);
            var mockDownloader = new MockDownloder(null);

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

        // ADH: 20.11.2011 was getting a bit confused over unexpected behavior in the unit tests when using mocks,
        // and am not 100% certain that I'm not perhaps screwing up something to do with the threads, statics, closures
        // etc, so am converting the method, to a lambda to avoid any doubt that some or other state is being "captured"
        // by the closure. (by closure I mean the new thread accesses a static method, which references a global, very
        // nasty to debug. This is an attempt to reduce (or help identify?) any possible errors of that nature.
        //+ ADH Update (a bit later on 20.11.2011): Seems that this did the trick and the tests are now working "as expected", without side effects.

        private static readonly Action<WaitTillComplete> WaitTillFilesAreDownloaded = (wtc) => {
            int cnt = 0;
            int cntMax = (wtc.SecondsToWait*1000) / 500;
            while (wtc.FilesProvider.ProcessQueueFilePaths.Count() !=  wtc.NumFiles)
            {
                _logger.Trace("\tfp.ProcessQueueFilePaths:{0}", wtc.FilesProvider.ProcessQueueFilePaths.Count());
                Thread.Sleep(500);
                if (cnt++ > cntMax) throw new Exception("timeout waiting " + wtc.SecondsToWait +" seconds for all the files to be downloaded.");
                wtc.FilesProvider.RefreshFiles();
            }
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

            var testData = new TestData(FilesProvider);

            Action createImportWaitForItToBeProcessed = () => {
                testData.CreateTestEnqueueRequestImportFile(Guid.NewGuid(), "Testfile.txt");
                WaitTillFilesAreDownloaded(new WaitTillComplete { FilesProvider = FilesProvider, NumFiles = 1,SecondsToWait = 4 });
            };

            var mockconsole = new MockConsole(
                new List<Line> { new Line(null, "download"), new Line(createImportWaitForItToBeProcessed, "exit") },
                new DateTimeWrapper(),
                true);

            var mockProcessor = new MockProcessFileCaller(null);

            var mockDownloader = new MockDownloder(null);
            
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
            Trace("And the file should be downloaded");
            Trace("Then there should be 0 files in the download queue");
            FilesProvider.DownloadQueueFilePaths.Count().Should().Be(0);
            Trace("and there should be 1 files in the process queue");
            FilesProvider.ProcessQueueFilePaths.Count().Should().Be(1);
        }

    }
}
