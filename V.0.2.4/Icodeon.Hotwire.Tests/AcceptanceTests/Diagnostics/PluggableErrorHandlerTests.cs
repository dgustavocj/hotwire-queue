using System;
using System.IO;
using System.Linq;
using System.Net;
using FluentAssertions;
using Icodeon.Hotwire.Framework.Scripts;
using Icodeon.Hotwire.TestFramework;
using Icodeon.Hotwire.TestFramework.Mocks;
using Icodeon.Hotwire.Tests.Internal;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.AcceptanceTests.Diagnostics
{
    [TestFixture(Category = "acceptanceTest", Description = "As a service provider, so that I can have timely and detailed information to fix errors and keep system running, I want to be able to plug in my own error handler to handle any errors")]
    public class PluggableErrorHandlerTests : FilesProviderAcceptanceTest
    {

        private TestData _testData;
        private MockErrorHandler _errorHandler1;
        private MockErrorHandler _errorHandler2;

        [SetUp]
        public void Setup()
        {
             _testData = new TestData(FilesProvider);
            FilesProvider.EmptyAllFolders();
            _errorHandler1 = new MockErrorHandler();
            _errorHandler2 = new MockErrorHandler();
        }

        [Test]
        public void ShouldHandleFileDownLoadScriptErrors()
        {
            TraceTitle("pluggable error handler should handle File download script errors");

            Trace("given one file in download queue");
            _testData.CopyFilesToDownloadQueueFolder(TestData.OneTestFile, ConsoleWriter);

            Trace("And a file download script that will throw an exception on the first file");
            var downloader = new MockDownloder(1);
            var script = new FileDownloaderScript(FilesProvider, downloader);
            script.DownloadException += _errorHandler1.HandleException;
            script.DownloadException += _errorHandler2.HandleException;
            _errorHandler1.Handled.Should().BeFalse();
            _errorHandler2.Handled.Should().BeFalse();

            Trace("when the script tries to download the files");
            script.Run(ConsoleWriter);

            Trace("then an exception should have been thrown by the downloader");
            downloader.HasThrown.Should().BeTrue();

            Trace("then both error handlers should have handled the error.");
            _errorHandler1.Handled.Should().BeTrue();
            _errorHandler2.Handled.Should().BeTrue();

        }

        [Test]
        public void ShouldHandleFileProcessScriptErrors()
        {
            TraceTitle("Should handle file process script errors.");

            Trace("Given a file proccessor script that will encounter a file process exception");
            var mockFileProcessCaller = new ThrowExceptionIfFileContainsXMockProcessFileCaller("numbers", "Whoops!");
            var processor = new FileProcessorScript(FilesProvider, mockFileProcessCaller);

            Trace("And two registered custom exception handlers");
            processor.ProcessException += _errorHandler1.HandleException;
            processor.ProcessException += _errorHandler2.HandleException;

            Trace("and three test files, 1 of which will be met with an exception");
            _testData.CopyFilesToProcessQueueFolder(TestData.ThreeTestImportFilesWithDownloadedContent, ConsoleWriter);
            FilesProvider.RefreshFiles();
            Assert.AreEqual(3, FilesProvider.ProcessQueueFilePaths.Count(), "should be 3 files waiting in the process queue.");

            Trace("When I run the file processor");
            Action action = ()=> processor.Run(ConsoleWriter);
            
            Trace("Then the Script should pass the exceptions to the handlers to be handled");
            action.ShouldNotThrow();
            _errorHandler1.Handled.Should().BeTrue();
            _errorHandler2.Handled.Should().BeTrue();
            
            Trace("and the script should have continued to process the other two files.");
            FilesProvider.RefreshFiles();
            Assert.AreEqual(0, FilesProvider.ProcessQueueFilePaths.Count(), "should be no files left in the process queue.");
            Assert.AreEqual(2, FilesProvider.ProcessedFilePaths.Count(), "all the test import files, except for 1 x exception file, should now be in processed");
            Assert.AreEqual(1, FilesProvider.ProcessErrorFilePaths.Count(), "only 1 error file should now be in process error.");
        }

        [Test]
        public void ShouldHandleModuleErrors()
        {
            TraceTitle("Ensure modules report (to any registered handlers) any unhandled exceptions");
            TraceTitle("Should be testable without requiring external website or other dependancies");

            Trace("Given a module that will respond with an IOException");
            var module = new MockModule();
            var configuration = ModuleConfigurationDTOFactory.GivenModuleConfigurationForMockModule();
            var streamingContext = new MockStreamingContext("http://localhost/throw/httpexception", configuration);

            Trace("And two handlers to handle the exceptions");
            module.ProcessRequestException += _errorHandler1.HandleException;
            module.ProcessRequestException += _errorHandler2.HandleException;
            _errorHandler1.Handled.Should().BeFalse();
            _errorHandler2.Handled.Should().BeFalse();

            Trace("When I call BeginRequest");
            module.BeginRequest(streamingContext);

            Trace("then the handler should receive the exception");
            _errorHandler1.Handled.Should().BeTrue();
            _errorHandler2.Handled.Should().BeTrue();

            Trace("and the module should return 301 (permanently moved)");
            streamingContext.HttpWriter.StatusCode.Should().Be((int)HttpStatusCode.MovedPermanently);

        }

        // not for now

        //[Test]
        //public void ShouldHandleAuthenticationErrors()
        //{
        //    throw new Exception("not implemented");
        //}


        [Test]
        public void HandlerShouldReceiveFullDetailsOfTheException()
        {
            // this is not a full coverage test, as this test should be applied for any object that exposes "on(X)exceptions", e.g. ModuleBase etc.
            TraceTitle("Handler should recieve full details of the exception");

            Trace("given one file in download queue");
            _testData.CopyFilesToDownloadQueueFolder(TestData.OneTestFile, ConsoleWriter);

            Trace("And a file download script that will throw an exception on the first file");
            var exceptionToThrow = new ArgumentException("argument message", new FileNotFoundException());
            var downloader = new MockDownloder(1,exceptionToThrow);
            var script = new FileDownloaderScript(FilesProvider, downloader);
            script.DownloadException += _errorHandler1.HandleException;
            script.DownloadException += _errorHandler2.HandleException;
            _errorHandler1.Handled.Should().BeFalse();
            _errorHandler2.Handled.Should().BeFalse();

            Trace("when the script tries to download the files");
            script.Run(ConsoleWriter);

            Trace("then an exception should have been thrown by the downloader");
            downloader.HasThrown.Should().BeTrue();

            Trace("then both error handlers should have handled the error.");
            _errorHandler1.Handled.Should().BeTrue();
            _errorHandler2.Handled.Should().BeTrue();
            
            Trace("And both handlers should have recieved the full exception details");
            _errorHandler1.ExceptionArgs.Exception.Should().BeSameAs(exceptionToThrow);
            _errorHandler2.ExceptionArgs.Exception.Should().BeSameAs(exceptionToThrow);

        }

    }
}
