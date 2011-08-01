using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
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
        protected override object ProcessRequest(HttpApplicationState applicationState, NameValueCollection queueParameters, Uri url, UriTemplateMatch match, IModuleEndpoint moduleConfig, IMediaInfo mediaInfo, IMapPath mapper, LoggerBase logger)
        {
            logger.Trace("QueueModule-> Action=" + moduleConfig.Action);
            logger.Trace("--------------------------------------");
            switch (moduleConfig.Action)
            {
                case ActionEnqueueRequest:
                    logger.Trace("\tParsing Name Value's from the input stream.");
                    

                    BeforeProcessFile(logger,queueParameters);

                    logger.Trace("\tValidating hotwire enqueue request required parameters.");
                    EnqueueRequestDTO.validateRequiredParameters(queueParameters);
                    logger.Trace("\t{0}",DeploymentEnvironment.CurrentBuildConfiguration);

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

                default: throw new ArgumentOutOfRangeException(moduleConfig.Action + " is not a supported action.");
            }

        }






        protected virtual void BeforeProcessFile(LoggerBase logger, NameValueCollection queueParameters)
        {
            // do nothing.
        }


    }
}
