using System;
using Icodeon.Hotwire.Framework.Scripts;

namespace Icodeon.Hotwire.Framework.Providers
{
    public interface IClientDownloader
    {
        FileDownloadResultDTO DownloadFileWithTiming(HotLogger logger, Uri uri, string downloadingFilePath);
    }
}