using System;
using System.Linq;
using System.Net;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Utils;
using Icodeon.Hotwire.Framework.Diagnostics;

namespace Icodeon.Hotwire.Framework.Modules
{
    public class EndpointMatch
    {
        public IModuleEndpoint Endpoint { get; set; }
        public UriTemplateMatch Match { get; set; }
    }

    public class EndpointRequestMatcher
    {
        public readonly IModuleConfiguration _configuration;

        public EndpointRequestMatcher(IModuleConfiguration configuration)
        {
            DebugContract.NotNullable(()=> configuration);
            _configuration = configuration;
        }

        public EndpointMatch MatchRequestOrNull(string httpMethod, Uri requestUrl)
        {
            // NB! Remember to optimise for speed as this get's called for every single request on clients server. It's potentially a website killer!
            if ((_configuration == null) || (!_configuration.Active)) return null;
            if (_configuration.MethodValidation == MethodValidation.beforeUriValidation)
            {
                if (!_configuration.Endpoints.Any(ep1 => ep1.HttpMethods.Any(ep2 => ep2.Contains(httpMethod)))) return null;
            }

            string rootUriString = string.Format("http://{0}:{1}/{2}", requestUrl.Host, requestUrl.Port, _configuration.RootServiceName);
            var rootUri = new Uri(rootUriString);
            var endpoint = _configuration.Endpoints.FirstOrDefault(ep => ep.Active && ep.UriTemplate.Match(rootUri, requestUrl) != null);
            if (endpoint == null) return null;

            if (_configuration.MethodValidation == MethodValidation.afterUriValidation)
            {
                if (!endpoint.HttpMethods.Any(ep2 => ep2.Contains(httpMethod)))
                {
                    throw new HttpModuleException(HttpStatusCode.MethodNotAllowed, "method '" + httpMethod + "', not allowed.");
                }
            }

            return new EndpointMatch
            {
                Endpoint = endpoint,
                Match = endpoint.UriTemplate.Match(rootUri, requestUrl)
            };
        }

    }
}
