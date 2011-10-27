using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Contracts.Enums;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Modules;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Security
{
    public class OAuthRequestAuthenticator : IAuthenticateRequest
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private IConsumerProvider _consumer;
        private IOAuthProvider _oAuthProvider;

        public OAuthRequestAuthenticator(IConsumerProvider consumer, IOAuthProvider oauthProvider)
        {
            _consumer = consumer;
            _oAuthProvider = oauthProvider;
        }

        public void AuthenticateRequest(ParsedBody parsedBody, NameValueCollection headers, string httpMethod, EndpointMatch endpointMatch)
        {
            if (endpointMatch.Endpoint.Security != SecurityType.oauth) throw new HttpModuleException(HttpStatusCode.InternalServerError, "Endpoint is not secured with oauth yet oauth authenticator is authenticating the request!");
            _logger.Trace("\tSecurity type is set to OAuth authentication.");

            //TODO: Fix later... this class is not really used, because the oauth provider uses a library that ignores all the passed in params and accesses them via the httpContext! aargh!!
            string key = parsedBody.Parameters[Constants.OAuth.oauth_consumer_key];
            if (key == null) throw new HttpModuleException(HttpStatusCode.Unauthorized, "The resource you requested requires that requests are oauth signed and no oauth consumer key was found.");

            _logger.Trace("\t{0}={1}.", Constants.OAuth.oauth_consumer_key, key);
            var secret = _consumer.GetConsumerSecret(key);
            bool valid = _oAuthProvider.IsValidSignatureForPost(key, secret, endpointMatch.Match.RequestUri, parsedBody.Parameters);
            if (!valid) throw new HttpModuleException(HttpStatusCode.Unauthorized, "Invalid OAuth signature. The resource you requested requires that requests are oauth signed.");
        }


    }
}
