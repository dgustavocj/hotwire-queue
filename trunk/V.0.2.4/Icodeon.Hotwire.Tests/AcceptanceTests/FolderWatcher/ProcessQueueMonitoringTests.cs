﻿using System;
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
    public class ProcessQueueMonitoringTests : FilesProviderUnitTest
    {

        [SetUp]
        public void Setup()
        {
        }


        [TearDown]
        public void TearDown()
        {
           FilesProvider.EmptyAllFolders();
        }


        [Test]
        public void ShouldBeAbleToHandleAfloodOfProcessFilesWithoutMissingAny()
        {
            TraceTitle("Should be able to handle a flood of import files (50) without missing any");
            Trace("Given a folderwatcher program");
            Trace("And a mock console that simulates a user who types 'download' then 'exit' at the console");
            Trace("When I create a 'flood' of enqueue requests (50 import files)");
            Trace("And I start download monitoring");

            var testData = new TestData(FilesProvider);

            Action createImportWaitForItToBeProcessed = () =>
            {
                for (int i = 0; i < 25; i++)
                {
                    testData.CreateTestProcessImportFileAndMockTestFile(Guid.NewGuid(), "Testfile.txt");
                    testData.CreateTestProcessImportFileAndMockTestFile(Guid.NewGuid(), "hello.txt");
                }
                int cnt = 0;
                while (FilesProvider.ProcessedFilePaths.Count() != 50)
                {
                    Thread.Sleep(500);
                    if (cnt++ > 8) throw new Exception("timeout waiting 4 seconds for all the files to be downloaded.");
                    FilesProvider.RefreshFiles();
                }
            };

            var mockconsole = new MockConsole(
                new List<MockConsole.Line> { new MockConsole.Line(null, "process"), new MockConsole.Line(createImportWaitForItToBeProcessed, "exit") },
                new DateTimeWrapper(),
                true);

            Trace("");
            Trace("----------------captured console output-----------------------------");
            FolderWatcherCommandProcessor.RunCommandProcessorUntilExit(false, FilesProvider, mockconsole, new DateTimeWrapper(), new MockProcessFileCaller(), new MockDownloder(null));
            Trace("----------------/captured console output----------------------------");
            FilesProvider.RefreshFiles();

            Trace("Then the FolderWatcher should start the processor and process all the files");
            Trace("Then there should be 0 files in the process queue");
            FilesProvider.ProcessQueueFilePaths.Count().Should().Be(0);
            Trace("and there should be 50 files in the processed queue");
            FilesProvider.ProcessedFilePaths.Count().Should().Be(50);
        }


    }
}