//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using FluentAssertions;
//using Icodeon.Hotwire.Framework;
//using Icodeon.Hotwire.Framework.Diagnostics;
//using Icodeon.Hotwire.Framework.Providers;
//using Icodeon.Hotwire.Framework.Scripts;
//using Icodeon.Hotwire.Framework.Utils;
//using Icodeon.Hotwire.TestFramework;
//using Icodeon.Hotwire.TestFramework.Mocks;
//using Icodeon.Hotwire.Tests.Internal;
//using NUnit.Framework;

//namespace Icodeon.Hotwire.Tests.AcceptanceTests.Diagnostics
//{
//    [TestFixture(Category = "acceptance-test", Description = "As a service provider, so that I can have timely and detailed information to fix errors and keep system running, I want to be able to plug in my own error handler to handle any errors")]
//    public class PluggableErrorHandlerTests : FilesProviderAcceptanceTest
//    {

//        private TestData _testData;

//        [SetUp]
//        public void Setup()
//        {
//          //  _testData = new TestData(FilesProvider);
//        }

//        [Test]
//        public void ShouldHandleFileDownLoadScriptErrors()
//        {
//            // this test can be moved to open source project as soon as the asp.net project is copied across.
//            TraceTitle("pluggable error handler should handle File download script errors ");

//            Trace("given one file in download queue");
//            var testData = new TestData(FilesProvider);
//            testData.CopyFilesToDownloadQueueFolder(TestData.OneTestFile, ConsoleWriter);

//            Trace("And two error handlers to handle FileDownload errors");
//            var errorHandlers = new [] {new MockErrorHandler(), new MockErrorHandler()};

//            Trace("And a file download script that will throw an exception on the first file");
//            var script = new FileDownloaderScript(FilesProvider, new MockDownloder(1));
            
//            Trace("when the script tries to download the files");
//            Action action = () => script.Run(Logger, ConsoleWriter);

//            Trace("then both error handlers should handle the error.");
//            errorHandlers[0].Handled.Should().BeTrue();
//            errorHandlers[1].Handled.Should().BeTrue();

//        }

//        public class MockErrorHandler : IExceptionHandler
//        {
//            public bool Handled { get; set; }

//            public void HandleException(Exception ex, Framework.Configuration.ePipeLineSection section)
//            {
//                Handled = true;
//            }
//        }

//        [Test]
//        public void ShouldHandleFileProcessScriptErrors()
//        {
//            throw new Exception("not implemented");
//        }

//        [Test]
//        public void ShouldHandleModuleErrors()
//        {
//            throw new Exception("not implemented");
//        }

//        [Test]
//        public void ShouldHandleAuthenticationErrors()
//        {
//            throw new Exception("not implemented");
//        }


//        [Test]
//        public void AllConfiguredErrorHandlersShouldBeCalledIfExceptionOccurs()
//        {
//            throw new Exception("not implemented");
//        }



//        [Test]
//        public void HandlerShouldReceiveFullDetailsOfTheException()
//        {

//            TraceTitle("Notification should include full details of the exception");

//            Trace("Given 'B21FCB05-1F88-4956-8FB6-3E5AA579B3F9-alphabet_textfile.import' is sitting in the process queue folder");
//            // ===================================================================================================================
//            _testData.CopyFilesToProcessQueueFolder(new[] { _testData.ImportFile }, ConsoleWriter);
//            FilesProvider.RefreshFiles();
//            Assert.AreEqual(1, FilesProvider.ProcessQueueFilePaths.Count());


//            Trace("and a file processor caller that will throw an exception for file containing 'alphabet_textfile.'");

//            throw new Exception("Not implemented.");
//            // =======================================================================================================
//            //var mockCaller = new DoesNotReleaseLockFileMockProcessFileCaller("_A180_2_", FilesProvider);

//            //Trace("and a FileProcessorScript");
//            //var processorScript = new FileProcessorScript(FilesProvider, Logger, new HttpClientProvider(), mockCaller);

//            //// !!!! FileStream locker = mockCaller.Locker;
//            //try
//            //{
//            //    Trace("When I start the file processor");
//            //    processorScript.Run(Logger, ConsoleWriter);

//            //    Trace("Then the processor processes all the files");
//            //    FilesProvider.RefreshFiles();
//            //    Trace("Then there should be only 1 [locked] file left in the processing queue.");
//            //    Assert.AreEqual(1, FilesProvider.ProcessingFilePaths.Count());

//            //    Trace("all the test import files, except for 1 x exception file, should now be in processed");
//            //    Assert.AreEqual(3, FilesProvider.ProcessedFilePaths.Count());

//            //    Trace("only 1 error file should now be in process error.");
//            //    Assert.AreEqual(1, FilesProvider.ProcessErrorFilePaths.Count());

//            //    // placeholder file should now exist in error folder!
//            //    // debug here and manually view this file!
//            //    // stop in other test and ensure it DOESNT exist!
//            //}
//            //finally
//            //{
//            //    // make sure our test doesnt crash and burn the test server ... without releasing this lock we wouldnt be able to do a cleanup.
//            //    if (mockCaller.Locker != null) mockCaller.Dispose();
//            //}


//        }

//        [Test]
//        public void NotificationToIncludeCopyOfOriginalEnqueueRequest()
//        {
//            throw new Exception("Not implemented.");
//        }

//        [Test]
//        public void NotificationToIncludeDownloadedFileIfExceptionOccursInFileProcessor()
//        {
//            throw new Exception("Not implemented.");
//        }


//    }
//}
