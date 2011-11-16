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
using NUnit.Framework;
using Line = Icodeon.Hotwire.TestFramework.Mocks.MockConsole.Line;

namespace Icodeon.Hotwire.Tests.AcceptanceTests.FolderWatcher
{
    [TestFixture]
    public class DownloadQueueMonitoringTests : UnitTest
    {
        private HotwireFilesProvider _filesProvider;

        [SetUp]
        public void Setup()
        {
            _filesProvider = HotwireFilesProvider.GetFilesProviderInstance();
            _filesProvider.EmptyAllFolders();
            _filesProvider.RefreshFiles();
        }


        [TearDown]
        public void TearDown()
        {
            _filesProvider.EmptyAllFolders();    
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
            
            var testData = new TestData(_filesProvider);

            Action createImportWaitForItToBeProcessed = () => {
                for (int i = 0; i < 25; i++) {
                    testData.CreateTestEnqueueRequestImportFile(Guid.NewGuid(), "Testfile.txt");
                    testData.CreateTestEnqueueRequestImportFile(Guid.NewGuid(), "hello.txt");
                }
                int cnt = 0;
                while (_filesProvider.ProcessQueueFilePaths.Count() != 50) {
                    Thread.Sleep(500);
                    if (cnt++ > 8) throw new Exception("timeout waiting 4 seconds for all the files to be downloaded.");
                    _filesProvider.RefreshFiles(); 
                }
            };

            var mockconsole = new MockConsole(
                new List<Line> { new Line(null, "download"), new Line(createImportWaitForItToBeProcessed, "exit") }, 
                new DateTimeWrapper(), 
                true);

            var mockProcessor = new MockProcessFileCaller(null);

            Trace("");
            Trace("----------------captured console output-----------------------------");
            FolderWatcherCommandProcessor.RunCommandProcessorUntilExit(false, _filesProvider, mockconsole, new DateTimeWrapper(),mockProcessor);
            Trace("----------------/captured console output----------------------------");
            _filesProvider.RefreshFiles();

            Trace("Then the FolderWatcher should start the downloader and download all the files");
            Trace("Then there should be 0 files in the download queue");
            _filesProvider.DownloadQueueFilePaths.Count().Should().Be(0);
            Trace("and there should be 50 files in the process queue");
            _filesProvider.ProcessQueueFilePaths.Count().Should().Be(50);
        }



    }
}
