using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Contracts.Enums;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.DAL;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Modules
{
    public class QueueModule : ModuleBase
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        protected override string GetConfigurationSectionName()
        {
            return QueueConfiguration.QueuesSectionName; 
        }

        public const string ActionEnqueueRequest = "ENQUEUE-REQUEST";

        public override IEnumerable<string> ActionNames
        {
            get { return new[] {ActionEnqueueRequest, ModuleBase.ActionVersion }; }
        }

        public override object ProcessRequest(ParsedContext context)
        {
            _logger.Trace("QueueModule-> Action=" + context.ModuleConfig.Action);
            _logger.Trace("--------------------------------------");
            switch (context.ModuleConfig.Action)
            {
                case ModuleBase.ActionVersion:
                    return AssemblyHelper.FrameworkVersion;

                case ActionEnqueueRequest:
                    BeforeProcessFile(context.RequestParameters);
                    _logger.Trace("\tValidating hotwire enqueue request required parameters.");
                    EnqueueRequestDTO.validateRequiredParameters(context.RequestParameters);
                    _logger.Trace("\t{0}", DeploymentEnvironment.CurrentBuildConfiguration);

                    var enqueueRequest = new EnqueueRequestDTO(context.RequestParameters)
                                             {
                                                 TransactionId = Guid.NewGuid().ToString()
                                             };

                    _logger.Trace("\transactionId = {0}", enqueueRequest.TransactionId);
                    var fileProvider = HotwireFilesProvider.GetFilesProviderInstance();

                    // if resource security type is "consumer" then sign the file url before writing to disk
                    if (enqueueRequest.ExtResourceLinkAuthoriseType==SecurityType.consumer.ToString())
                    {
                        var signer = new ProviderFactory().CreateUrlSignatureProvider();
                        enqueueRequest.ExtResourceLinkContent = signer.SignUrl(new Uri(enqueueRequest.ExtResourceLinkContent),
                                           enqueueRequest.
                                               ToUnderScoreIcodeonCCPNamedNameValueCollectionPlusExtraHotwireParamsAndAnyExtraParamsPostedByClient
                                               ()).ToString();
                    }

                    var dal = new QueueDal(fileProvider);

                    


                    dal.Save(enqueueRequest, QueueStatus.QueuedForDownloading);
                    var queuedResource = new QueuedResource
                                             {
                                                 TrackingNumber = enqueueRequest.GetTrackingNumber()
                                             };
                    _logger.Trace("\tTRACKING-NUMBER : {0}", queuedResource.TrackingNumber);
                    return queuedResource;

                default: throw new ArgumentOutOfRangeException(context.ModuleConfig.Action + " is not a supported QueueModule action.");
            }

        }






        protected virtual void BeforeProcessFile(NameValueCollection queueParameters)
        {
            // do nothing.
        }


    }
}
