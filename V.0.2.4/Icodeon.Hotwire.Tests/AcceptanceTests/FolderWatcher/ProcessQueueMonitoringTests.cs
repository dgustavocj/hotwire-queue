using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FluentAssertions;
using Icodeon.Hotwire.Framework.FolderWatcher;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Utils;
using Icodeon.Hotwire.TestFramework;
using Icodeon.Hotwire.TestFramework.Mocks;
using Icodeon.Hotwire.Tests.Internal;
using NUnit.Framework;

namespace Icodeon.Hotwire.Framework.OUTests.AcceptanceTests.FolderWatcher
{
    [TestFixture]
    public class ProcessQueueMonitoringTests : UnitTest
    {
        private HotwireFilesProvider _filesProvider;

        [SetUp]
        public void Setup()
        {
            _filesProvider = HotwireFilesProvider.GetFilesProviderInstance();
            _filesProvider.EmptyAllFoldersCreateIfNotExist();
            _filesProvider.RefreshFiles();
        }


        [TearDown]
        public void TearDown()
        {
           _filesProvider.EmptyAllFoldersCreateIfNotExist();    
        }


        [Test]
        public void ShouldBeAbleToHandleAfloodOfProcessFilesWithoutMissingAny()
        {
            TraceTitle("Should be able to handle a flood of import files (50) without missing any");
            Trace("Given a folderwatcher program");
            Trace("And a mock console that simulates a user who types 'download' then 'exit' at the console");
            Trace("When I create a 'flood' of enqueue requests (50 import files)");
            Trace("And I start download monitoring");

            var testData = new TestData(_filesProvider);

            Action createImportWaitForItToBeProcessed = () =>
            {
                for (int i = 0; i < 25; i++)
                {
                    testData.CreateTestProcessImportFileAndMockTestFile(Guid.NewGuid(), "Testfile.txt");
                    testData.CreateTestProcessImportFileAndMockTestFile(Guid.NewGuid(), "hello.txt");
                }
                int cnt = 0;
                while (_filesProvider.ProcessedFilePaths.Count() != 50)
                {
                    Thread.Sleep(500);
                    if (cnt++ > 8) throw new Exception("timeout waiting 4 seconds for all the files to be downloaded.");
                    _filesProvider.RefreshFiles();
                }
            };

            var mockconsole = new MockConsole(
                new List<MockConsole.Line> { new MockConsole.Line(null, "process"), new MockConsole.Line(createImportWaitForItToBeProcessed, "exit") },
                new DateTimeWrapper(),
                true);

            Trace("");
            Trace("----------------captured console output-----------------------------");
            FolderWatcherCommandProcessor.RunCommandProcessorUntilExit(false, _filesProvider, mockconsole, new DateTimeWrapper(), new MockProcessFileCaller());
            Trace("----------------/captured console output----------------------------");
            _filesProvider.RefreshFiles();

            Trace("Then the FolderWatcher should start the processor and process all the files");
            Trace("Then there should be 0 files in the process queue");
            _filesProvider.ProcessQueueFilePaths.Count().Should().Be(0);
            Trace("and there should be 50 files in the processed queue");
            _filesProvider.ProcessedFilePaths.Count().Should().Be(50);
        }


    }
}
