//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Icodeon.Hotwire.Framework;
//using Icodeon.Hotwire.Framework.Providers;
//using Icodeon.Hotwire.TestFramework;
//using Icodeon.Hotwire.Tests.Internal;
//using NUnit.Framework;

//namespace Icodeon.Hotwire.Tests.AcceptanceTests.Diagnostics
//{
//    [TestFixture("acceptance-test","As a service provider, so that I can have timely and detailed information to fix errors and keep system running, I want to be notified whenever a folderwatcher error ocurs")]
//    public class FolderWatcherErrorHandlingAndNotification : FilesProviderAcceptanceTest
//    {

//        private TestData _testData;

//        [SetUp]
//        public void Setup()
//        {
//            _testData = new TestData(HotwireFilesProvider.GetFilesProviderInstance(HotLogger.NullLogger));
//        }

//        [Test]
//        public void AllImplementationsOfIHandleErrorShouldBeCalledIfExceptionOccurs()
//        {
//            throw new NotImplementedException();
//        }

//        [Test]
//        public void NotificationShouldIncludeFullDetailsOfTheException()
//        {

//            TraceTitle("Notification should include full details of the exception");

//            Trace("Given 'B21FCB05-1F88-4956-8FB6-3E5AA579B3F9-alphabet_textfile.import' is sitting in the process queue folder");
//            // ===================================================================================================================
//            _testData.CopyFilesToProcessQueueFolder(new[] { _testData.ImportFile }, ConsoleWriter);
//            FilesProvider.RefreshFiles();
//            Assert.AreEqual(1, FilesProvider.ProcessQueueFilePaths.Count());


//            Trace("and a file processor caller that will throw an exception for file containing 'alphabet_textfile.'");
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
//            throw new NotImplementedException();
//        }

//        [Test]
//        public void NotificationToIncludeDownloadedFileIfExceptionOccursInFileProcessor()
//        {
//            throw new NotImplementedException();
//        }

    
//    }
//}
