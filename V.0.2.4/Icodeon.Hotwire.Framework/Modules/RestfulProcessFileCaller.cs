using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Diagnostics;
using NLog;

namespace Icodeon.Hotwire.Framework.Modules
{
    public class RestfulProcessFileCaller : ProcessFileCallerBase
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IHttpClientProvider _httpClient;
        private readonly ProcessFileScriptSection _processFileConfiguration;


        public RestfulProcessFileCaller(IHttpClientProvider httpClient, ProcessFileScriptSection processFileConfiguration)
        {
            _logger.Trace("RestfulProcessFileCaller({0},{1})",httpClient,processFileConfiguration);
            _httpClient = httpClient;
            _processFileConfiguration = processFileConfiguration;
        }

        public override void CallProcessFileWaitForComplete(string trackingNumber)
        {
            _logger.Info("CallProcessFileWaitForComplete({0})",trackingNumber);
            string endpoint = _processFileConfiguration.GetEndpoint(trackingNumber);
            _logger.Debug("endpoint:{0}",endpoint);
            var endpointUri = new Uri(endpoint);
            _logger.Trace("about to call _httpClient.GetAndEnsureStatusIsSuccessful(...)");
            _httpClient.GetAndEnsureStatusIsSuccessful(endpointUri);
        }
    }
}
