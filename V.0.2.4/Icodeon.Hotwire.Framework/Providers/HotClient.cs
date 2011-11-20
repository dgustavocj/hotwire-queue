using System;
using System.IO;
using System.Net;
using System.Text;
using Icodeon.Hotwire.Framework.Scripts;
using NLog;

namespace Icodeon.Hotwire.Framework.Providers
{
    public class HotClient : WebClient, IClientDownloader
    {
        public TimeSpan Timeout { get; set; }

        private CookieContainer cookieContainer = new CookieContainer();
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public HotClient()
        {
            Timeout = TimeSpan.FromSeconds(600);
        }

        public HotClient(TimeSpan timespan)
        {
            Timeout = timespan;
            if (timespan.Milliseconds > int.MaxValue)
                throw new ArgumentOutOfRangeException("timespan too big, cannot exceed " + int.MaxValue + " milliseconds.");
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            if (request == null) return null;
            // turn on cookie support otherwise custom authentication ( e.g. 303 redirects that bounce you to forms authentication ) will not work if they use cookies.

            request.CookieContainer = cookieContainer;

            // set decompression for GZip so that DownloadProgressChanges will fire?
            request.AutomaticDecompression = DecompressionMethods.GZip;


            request.Timeout = (int)Timeout.TotalMilliseconds;
            return request;
        }

        public FileDownloadResultDTO DownloadFileWithTiming(Uri uri, string downloadingFilePath)
        {
            // ignoring timeout for now
            var startTime = DateTime.Now;
            using (var client = new HotClient())
            {
                // not using stopwatch to measure the time to download the file 
                // see here why not: http://kristofverbiest.blogspot.com/2008/10/beware-of-stopwatch.html
                // also, downloads take a fair time, (few seconds) so DateTime is best
                _logger.Trace("Downloading: {0,-20}", downloadingFilePath);
                _logger.Debug("External resource link (uri):'{0}'",uri);
                client.DownloadFile(uri, downloadingFilePath);
                // finished downloading move files to processQueue folder
                double seconds = DateTime.Now.Subtract(startTime).TotalSeconds;
                var fi = new FileInfo(downloadingFilePath);
                long bytes = fi.Length;
                double kb = bytes / 1024;
                double kbPerSec = kb / seconds;
                var retval = new FileDownloadResultDTO()
                                    {
                                        DownloadedFile = new FileInfo(downloadingFilePath),
                                        KbPerSec = kbPerSec,
                                        Kilobytes = kb,
                                        Seconds = seconds
                                    };
                var msg = string.Format(" {0,7}Kb in {1,5:0.00} seconds {2,7:0.00}Kb/sec", kb, seconds, kbPerSec);
                _logger.Trace(msg);
                return retval;
            }
        }

        public static GetResultDTO GetWithTimeout(Uri uri, TimeSpan timeSpan)
        {
            var start = DateTime.Now;
            var request = new HotClient(timeSpan).GetWebRequest(uri);
            var response = (HttpWebResponse)request.GetResponse();
            string result = new StreamReader(response.GetResponseStream(),Encoding.UTF8).ReadToEnd();
            int milliseconds = (int)DateTime.Now.Subtract(start).TotalMilliseconds;
            return new GetResultDTO()
                       {
                           MilliSeconds = milliseconds,
                           ResponseText = result,
                           StatusCode = (int) response.StatusCode,
                           ContentType = response.ContentType
                       };
        }

    }
}
