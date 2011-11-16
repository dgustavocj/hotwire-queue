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
    public class AutostartMonitoringTests : UnitTest
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


        [Test]
        public void ShouldDownloadAndThenProcessEnqueuedRequests()
        {
            TraceTitle("Should download and then process an enqueued request");
            Trace("Given a folderwatcher program");
            Trace("And a mock console that simulates a user who types 'download' then 'exit' at the console");
            Trace("When an enqueue request is detected");
            Trace("And I start both download and process monitoring");

            var testData = new TestData(_filesProvider);

            Action createImportWaitForItToBeProcessed = () =>
            {
                testData.CreateTestEnqueueRequestImportFile(Guid.NewGuid(), "Testfile.txt");
                testData.CreateTestEnqueueRequestImportFile(Guid.NewGuid(), "hello.txt");
                testData.CreateTestEnqueueRequestImportFile(Guid.NewGuid(), "Testfile.txt");
                int cnt = 0;
                while (_filesProvider.ProcessedFilePaths.Count() != 3)
                {
                    Thread.Sleep(500);
                    if (cnt++ > 8) throw new Exception("timeout waiting 4 seconds for all the files to be downloaded and processed.");
                    _filesProvider.RefreshFiles();
                }
            };

            var mockconsole = new MockConsole(
                new List<Line> { new Line(null, "start"), new Line(createImportWaitForItToBeProcessed, "exit") },
                new DateTimeWrapper(),
                true);

            var mockProcessor = new MockProcessFileCaller(null);

            Trace("");
            Trace("----------------captured console output-----------------------------");
            FolderWatcherCommandProcessor.RunCommandProcessorUntilExit(false, _filesProvider, mockconsole, new DateTimeWrapper(), mockProcessor);
            Trace("----------------/captured console output----------------------------");
            _filesProvider.RefreshFiles();

            Trace("Then the the files should be downloaded");
            _filesProvider.DownloadQueueFilePaths.Count().Should().Be(0);
            _filesProvider.DownloadErrorFilePaths.Count().Should().Be(0);

            Trace("And the files should be processed");
            _filesProvider.ProcessQueueFilePaths.Count().Should().Be(0);
            _filesProvider.ProcessErrorFilePaths.Count().Should().Be(0);
            _filesProvider.ProcessingFilePaths.Count().Should().Be(0);
            _filesProvider.ProcessedFilePaths.Count().Should().Be(3);
        }

    }
}
