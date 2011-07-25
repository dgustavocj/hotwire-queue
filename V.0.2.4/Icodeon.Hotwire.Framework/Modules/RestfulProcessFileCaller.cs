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
        private readonly ProcessFileSection _processFileSection;
        

        public RestfulProcessFileCaller(IHttpClientProvider httpClient, ProcessFileSection processFileSection)
        {
            _httpClient = httpClient;
            _processFileSection = processFileSection;
        }

        public override void CallProcessFileWaitForComplete(string trackingNumber)
        {
            string endpoint = ProcessFileSection.ReadConfig().GetEndpoint(trackingNumber);
            var endpointUri = new Uri(endpoint);
            _httpClient.GetAndEnsureStatusIsSuccessful(endpointUri);
        }
    }
}
