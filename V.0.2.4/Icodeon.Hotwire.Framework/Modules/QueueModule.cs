using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Repository;
using Icodeon.Hotwire.Framework.Security;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Modules
{
    public class QueueModule : ModuleBase
    {

        protected override string ConfigurationSectionName
        {
            get { return Constants.Configuration.QueuesSectionName; }
        }

        public const string ActionEnqueueRequest = "ENQUEUE-REQUEST";

        public override IEnumerable<string> ActionNames
        {
            get { return new[] {ActionEnqueueRequest}; }
        }


        //TODO: Move all the request context to a rest module context DTO?
        // it's not a too massive list and I like seeing there here (at least for now) as it's a reminder of what I have to work with,do or dont need.
        protected override object ProcessRequest(HttpApplicationState applicationState, Stream inputStream,Uri url, UriTemplateMatch match, IModuleEndpoint config, IMediaInfo mediaInfo, Utils.IMapPath mapper, NLog.Logger logger)
        {
            logger.Trace("QueueModule-> Action=" + config.Action);
            logger.Trace("--------------------------------------");
            switch (config.Action)
            {
                case ActionEnqueueRequest:
                    logger.Trace("\tParsing Name Value's from the input stream.");
                    var queueParameters = inputStream.ParseNameValues();

                    BeforeProcessFile(logger,queueParameters);

                    logger.Trace("\tValidating hotwire enqueue request required parameters.");
                    EnqueueRequestDTO.validateRequiredParameters(queueParameters);
                    logger.Trace("\t{0}",DeploymentEnvironment.IsDEBUG ? "DEBUG BUILD." : "RELEASE BUILD.");
                    if (config.Security==SecurityType.oauth)
                    {
                        logger.Trace("\tSecurity type is set to OAuth authentication.");
                        string key = queueParameters[Constants.OAuth.oauth_consumer_key];
                        logger.Trace("\t{0}={1}.", Constants.OAuth.oauth_consumer_key, key);
                        if (key==null)
                        {
                            throw new HttpModuleException(HttpStatusCode.Unauthorized, "The resource you requested requires that requests are oauth signed.");
                        }

                        if (DeploymentEnvironment.IsDEBUG)
                        {
                            CheckConsumerKeyIsDevKey(key, logger);
                        }
                        else
                        {
                            CheckConsumerKeyIsHardCodedPartners(key, logger);
                        }
                            
                        
                    }
                    var enqueueRequest = new EnqueueRequestDTO(queueParameters)
                                             {
                                                 TransactionId = Guid.NewGuid().ToString()
                                             };
                    
                    logger.Trace("\transactionId = {0}",enqueueRequest.TransactionId);
                    var fileProvider = HotwireFilesProvider.GetFilesProviderInstance(logger);
                    var dal = new QueueDal(fileProvider, logger);
                    dal.Save(enqueueRequest, QueueStatus.QueuedForDownloading);
                    var queuedResource = new QueuedResource()
                                             {
                                                 TrackingNumber = enqueueRequest.GetTrackingNumber()
                                             };
                    logger.Trace("\tTRACKING-NUMBER : {0}", queuedResource.TrackingNumber);
                    return queuedResource;

                default: throw new ArgumentOutOfRangeException(config.Action + " is not a supported action.");
            }

        }


        private bool ValidateOauthSignature(Logger logger, string consumerKey, NameValueCollection queueParameters, Uri requestUrl)
        {
            IConsumerProvider consumer = new ProviderFactory(logger).CreateConsumerProvider();
            var secret = consumer.GetConsumerSecret(consumerKey);
            // instead of hard coding it to quickAuth, we can ask the objectfactory/structureMap to 
            // automatically scan the assemblies and look this up for us?


            // TODO: code below also candidate for ninject ( passing in two parameters, and ninject fills in the missing one)
            var oauthProvider = new ProviderFactory(logger).CreateOauthProvider();
            var oauth = new QuickAuth(consumerKey, secret, oauthProvider);
            bool isvalid = oauth.ValidateSignature(requestUrl, queueParameters);
            return isvalid;
        }

        private void CheckConsumerKeyIsDevKey(string key,Logger logger)
        {
            
            if (!key.ToLowerInvariant().Equals("key"))
                throw new HttpModuleException(HttpStatusCode.Unauthorized,"Invalid oauth key, key was "+ key);
            else
                logger.Trace("The oauth consumer key is the correct key for DEBUG build.");
        }

        private void CheckConsumerKeyIsHardCodedPartners(string key, Logger logger)
        {
            //TODO: Move message texts to resource files
            if (!key.ToLowerInvariant().Equals(Constants.TemporaryKeyAndSecretLookups.PartnerConsumerKey)) 
                throw new HttpModuleException(HttpStatusCode.Unauthorized, "Invalid oauth key, key was " + key);
            else
                logger.Trace("The oauth consumer key is the correct key for RELEASE build.");
        }

        protected virtual void BeforeProcessFile(Logger logger, NameValueCollection queueParameters)
        {
            // do nothing.
        }


    }
}
