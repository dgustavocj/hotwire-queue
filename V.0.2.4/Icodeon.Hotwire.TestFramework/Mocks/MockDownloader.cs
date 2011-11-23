using System;
using System.IO;
using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Scripts;
using NLog;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class MockDownloader : IClientDownloader
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public bool HasThrown { get; set; }
        private int _cntFiles;
        private int? _throwOnFileNo;
        private readonly Exception _exceptionToThrow;

        private FileDownloadResultDTO createFakeDownload(string path)
        {
            _logger.Trace("MockDownloder: Creating fake download:'{0}'", path);
            File.WriteAllText(path, "hello world.");
            return new FileDownloadResultDTO
                       {
                           DownloadedFile = new FileInfo(path),
                           KbPerSec = 120,
                           Kilobytes = 12,
                           Seconds = 0.1
                       };
        }

        public MockDownloader(int? throwOnFileNo, Exception exceptionToThrow)
        {
            _throwOnFileNo = throwOnFileNo;
            _exceptionToThrow = exceptionToThrow;
        }

        public MockDownloader() : this(null)
        {

        }

        public MockDownloader(int? throwOnFileNo) : this(throwOnFileNo, new FileNotFoundException("file not found."))
        {
            _throwOnFileNo = throwOnFileNo;
        }

        public FileDownloadResultDTO DownloadFileWithTiming(Uri uri, string downloadingFilePath)
        {
            _cntFiles++;
            if (_cntFiles == (_throwOnFileNo ?? -1))
            {
                HasThrown = true;
                throw _exceptionToThrow;
            }
            return createFakeDownload(downloadingFilePath);
        }
    }
}