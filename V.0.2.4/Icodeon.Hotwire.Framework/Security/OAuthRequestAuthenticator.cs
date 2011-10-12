using System;
using System.Collections.Specialized;
using System.Net;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
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

        public void AuthenticateRequest(NameValueCollection requestParameters, NameValueCollection headers, string httpMethod, EndpointMatch endpointMatch)
        {
            _logger.Trace("\tSecurity type is set to OAuth authentication.");
            string key = requestParameters[Constants.OAuth.oauth_consumer_key];
            _logger.Trace("\t{0}={1}.", Constants.OAuth.oauth_consumer_key, key);
            if (key == null)
            {
                throw new HttpModuleException(HttpStatusCode.Unauthorized, "The resource you requested requires that requests are oauth signed.");
            }

//#if DEBUG
//                CheckConsumerKeyIsDevKey(key);
//#endif
//#if RELEASE
//                CheckConsumerKeyIsHardCodedPartners(key);
//#endif

            bool valid  = ValidateOauthSignature(key, requestParameters, endpointMatch.Match.RequestUri);
            if (!valid) throw new HttpModuleException(HttpStatusCode.Unauthorized, "Invalid OAuth signature. The resource you requested requires that requests are oauth signed.");
        }



        private void CheckConsumerKeyIsDevKey(string key)
        {

            if (!key.ToLowerInvariant().Equals("key"))
                throw new HttpModuleException(HttpStatusCode.Unauthorized, "Invalid oauth key, key was " + key);
            else
                _logger.Trace("The oauth consumer key is the correct key for DEBUG build.");
        }

        private void CheckConsumerKeyIsHardCodedPartners(string key)
        {
            //TODO: Move message texts to resource files
            if (!key.ToLowerInvariant().Equals(Constants.TemporaryKeyAndSecretLookups.PartnerConsumerKey))
                throw new HttpModuleException(HttpStatusCode.Unauthorized, "Invalid oauth key, key was " + key);
            else
                _logger.Trace("The oauth consumer key is the correct key for RELEASE build.");
        }

        // TODO: dont use factories here...pass in as a dependancy!
        private bool ValidateOauthSignature(string consumerKey, NameValueCollection queueParameters, Uri requestUrl)
        {
            IConsumerProvider consumer = new ProviderFactory().CreateConsumerProvider();
            var secret = consumer.GetConsumerSecret(consumerKey);
            var oauthProvider = new ProviderFactory().CreateOauthProvider();
            bool isvalid = oauthProvider.IsValidSignatureForPost(consumerKey, secret, requestUrl, queueParameters);
            return isvalid;
        }

    }
}
