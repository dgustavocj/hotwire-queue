using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;

namespace Icodeon.Hotwire.Framework.Modules
{
    public class RestfulProcessFileCaller : ProcessFileCallerBase
    {
        private readonly IHttpClientProvider _httpClient;
        private readonly ProcessFileScriptSection _processFileConfiguration;


        public RestfulProcessFileCaller(IHttpClientProvider httpClient, ProcessFileScriptSection processFileConfiguration)
        {
            _httpClient = httpClient;
            _processFileConfiguration = processFileConfiguration;
        }

        public override void CallProcessFileWaitForComplete(string trackingNumber)
        {
            string endpoint = _processFileConfiguration.GetEndpoint(trackingNumber);
            var endpointUri = new Uri(endpoint);
            _httpClient.GetAndEnsureStatusIsSuccessful(endpointUri);
        }
    }
}
