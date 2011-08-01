using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Scripts;
using Icodeon.Hotwire.Framework.Utils;
using Icodeon.Hotwire.TestFramework;
using NUnit.Framework;

namespace Icodeon.Hotwire.Tests.UnitTests
{
    [TestFixture]
    public class FileDownloaderScriptTests : UnitTest
    {

        //[Test]
        public void FileNotFoundShouldNotStopDownloaderScript()
        {
            // given a download script and a file that does not exist
            var httpClient = new HotClient();
            var downloadScript = new FileDownloaderScript(new MockDownloaderFilesProvider(), httpClient);
            throw new Exception("not implemented yet!");
            // when the script tries to download the file
            // then a file not found exception should be logged 
            // and the script should continue
        }

        internal class MockDownloaderFilesProvider : IDownloaderFilesProvider
        {

            public IEnumerable<string> DownloadQueueFilePaths
            {
                get { throw new NotImplementedException(); }
            }

            public string DownloadingFolderPath
            {
                get { throw new NotImplementedException(); }
            }

            public string ProcessQueueFolderPath
            {
                get { throw new NotImplementedException(); }
            }

            public string DownloadErrorFolderPath
            {
                get { throw new NotImplementedException(); }
            }

            public void MoveFileAndSettingsFileFromDownloadingFolderToDownloadErrorFolderWriteExceptionFile(string trackingNumber, Exception ex)
            {
                throw new NotImplementedException();
            }

            public void MoveImportFileFromDownloadQueueuToDownloading(string importFileName)
            {
                throw new NotImplementedException();
            }

            public void RefreshFiles()
            {
                throw new NotImplementedException();
            }

            public QueueStatus GetStatusByImportFileName(string importFileName)
            {
                throw new NotImplementedException();
            }
        }

    }

    
}
