using System;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Serialization;
using NLog;

namespace Icodeon.Hotwire.Framework
{
    public class FileProcessorPinger
    {
        private readonly Logger _logger;
        private readonly Uri _endpoint;

        public FileProcessorPinger(NLog.Logger logger, Uri endpoint)
        {
            _logger = logger;
            _endpoint = endpoint;
        }

        public ProcessFileRequestResult PingProcessFile(string file)
        {
            _logger.Info("PingProcessFile {0}", file);
            var client = new ProviderFactory(_logger).CreateHttpClient();
            Uri requestUri = new Uri(string.Format("{0}/{1}",_endpoint, file));
            _logger.Info("Making a GET request to {0}", requestUri.ToString());
            string responsetext = client.GetResponseAsStringEnsureStatusIsSuccessful(requestUri);
            _logger.Info("response:{0}", responsetext);
            return JSONHelper.Deserialize<ProcessFileRequestResult>(responsetext);
        }


    }
}
