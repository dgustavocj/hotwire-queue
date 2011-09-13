using System;
using Icodeon.Hotwire.Framework.Scripts;

namespace Icodeon.Hotwire.Framework.Providers
{
    public interface IClientDownloader
    {
        FileDownloadResultDTO DownloadFileWithTiming(Uri uri, string downloadingFilePath);
    }
}