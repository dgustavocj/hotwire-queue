using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Repository;
using Icodeon.Hotwire.Framework.Utils;

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
            get { return new[] {ActionEnqueueRequest, ModuleBase.ActionVersion }; }
        }

        public override object ProcessRequest(ParsedContext context)
        {
            context.Logger.Trace("QueueModule-> Action=" + context.ModuleConfig.Action);
            context.Logger.Trace("--------------------------------------");
            switch (context.ModuleConfig.Action)
            {
                case ModuleBase.ActionVersion:
                    return AssemblyHelper.FrameworkVersion;

                case ActionEnqueueRequest:
                    BeforeProcessFile(context.Logger, context.RequestParameters);
                    context.Logger.Trace("\tValidating hotwire enqueue request required parameters.");
                    EnqueueRequestDTO.validateRequiredParameters(context.RequestParameters);
                    context.Logger.Trace("\t{0}", DeploymentEnvironment.CurrentBuildConfiguration);

                    var enqueueRequest = new EnqueueRequestDTO(context.RequestParameters)
                                             {
                                                 TransactionId = Guid.NewGuid().ToString()
                                             };

                    context.Logger.Trace("\transactionId = {0}", enqueueRequest.TransactionId);
                    var fileProvider = HotwireFilesProvider.GetFilesProviderInstance(context.Logger);
                    var dal = new QueueDal(fileProvider, context.Logger);
                    dal.Save(enqueueRequest, QueueStatus.QueuedForDownloading);
                    var queuedResource = new QueuedResource
                                             {
                                                 TrackingNumber = enqueueRequest.GetTrackingNumber()
                                             };
                    context.Logger.Trace("\tTRACKING-NUMBER : {0}", queuedResource.TrackingNumber);
                    return queuedResource;

                default: throw new ArgumentOutOfRangeException(context.ModuleConfig.Action + " is not a supported QueueModule action.");
            }

        }






        protected virtual void BeforeProcessFile(LoggerBase logger, NameValueCollection queueParameters)
        {
            // do nothing.
        }


    }
}
