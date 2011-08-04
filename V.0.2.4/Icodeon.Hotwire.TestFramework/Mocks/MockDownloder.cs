using System;
using System.IO;
using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Scripts;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class MockDownloder : IClientDownloader
    {
        private int _cntFiles;
        private int? _throwOnFileNo;

        private FileDownloadResultDTO createFakeDownload(string path)
        {
            File.WriteAllText(path, "hello world.");
            return new FileDownloadResultDTO
                       {
                           DownloadedFile = new FileInfo(path),
                           KbPerSec = 40,
                           Kilobytes = 12,
                           Seconds = 2
                       };
        }
        public MockDownloder(int? throwOnFileNo)
        {
            _throwOnFileNo = throwOnFileNo;
        }

        public FileDownloadResultDTO DownloadFileWithTiming(HotLogger logger, Uri uri, string downloadingFilePath)
        {
            _cntFiles++;
            if (_cntFiles == (_throwOnFileNo ?? -1)) throw new Exception(downloadingFilePath + " not found.");
            return createFakeDownload(downloadingFilePath);
        }
    }
}