using System;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Serialization;

namespace Icodeon.Hotwire.Framework
{
    public class FileProcessorPinger
    {
        private readonly LoggerBase _logger;
        private readonly IHttpClientProvider _httpClient;

        public FileProcessorPinger(LoggerBase logger, IHttpClientProvider httpClient)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public ProcessFileRequestResult PingProcessFile(Uri endpoint,string file)
        {
            _logger.Info("PingProcessFile {0}", file);
            var requestUri = new Uri(string.Format("{0}/{1}", endpoint, file));
            _logger.Info("Making a GET request to {0}", requestUri.ToString());
            string responsetext = _httpClient.GetResponseAsStringEnsureStatusIsSuccessful(requestUri);
            _logger.Info("response:{0}", responsetext);
            return JSONHelper.Deserialize<ProcessFileRequestResult>(responsetext);
        }


    }
}
