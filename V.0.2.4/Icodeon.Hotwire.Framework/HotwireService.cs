using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework
{

    // NB! I have to think about threading because the ServiceHost factory is configured to use a singleton that handles each request!

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, Namespace = Constants.Namespaces.ICODEON_HOTWIRE_BETA_V0_2)]
    
    // hotwire can't use current oauth library with WCF self hosted, and must be hosted in IIS in order to access HttpContext.Current.Request!

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class HotwireService : IHotwireService, IHotwireServiceEvents
    {

        private static LoggerBase logger = new HotLogger(NLog.LogManager.GetCurrentClassLogger());
        //ADH: looks like lastFile is never used, need to check.
        private static int lastFile = 1;



        public QueuedResource Enqueue(string module, Stream body)
        {
            // todo validate module is configured
            _serviceRequesting("Enqueue");
            var bodyParameters = body.ParseNameValues();
            _enqueuing("Enqueue", module, bodyParameters);
            var er = new EnqueueRequestDTO(bodyParameters);
            try
            {
                logger.Trace("key=***, secret=****** [ blanked out while log is public ]");
                var validator = new OAuthHelper("key", "secret", new TimeSpan(0, 0, 5));

                validator.ValidateHttpRequestHasbeenOAuthSigned_1_0(bodyParameters);
                
                logger.Trace("Request validated. Has been Oauth 1.0 signed.");

                logger.TraceParameters(er.ToNameValueCollectionIncludingExtraParamsPostedByConsumer());
                Guid transactionId = Guid.NewGuid();

                var filename = Path.GetFileName(er.ExtResourceLinkContent);
                string trackingNumber = string.Format("{0}_{1}.import", transactionId, filename);
                var queuedItem = new QueuedResource(trackingNumber);

                logger.Trace("Resource added to the queue. Tracking number:{0}", transactionId);

                //var writer = new ResponsableHttpContextWriter();
                //writer.WriteJsonResponse(queuedItem);
                return queuedItem;
                
            } catch(Exception ex)
            {
                logger.Error(ex.Message,ex);
                throw;
            } 

        }


        public string EnqueueStream(Stream body, string module)
        {
            _serviceRequesting("EnqueueStream");
            var parameters = body.ParseNameValues();
            return string.Join(",", parameters.AllKeys.Select(k => k + ":" + parameters[k]).ToArray());
        }



        public string VersionFramework()
        {
            _serviceRequesting("VersionFramework");
            //
            var fv = AssemblyHelper.FrameworkVersion;
            
            logger.Info("<- {0}", fv);
            return fv;
        }

        public string VersionServiceHost()
        {
            _serviceRequesting("VersionServiceHost");
            //
            var sv = AssemblyHelper.ServiceVersion;
            
            logger.Info("<- {0}", sv);
            return sv;
        }

        private void _serviceRequesting(string methodName)
        {
            logger.Info("-> {0}()",methodName);

            var request = WebOperationContext.Current.IncomingRequest;
            string method = request.Method;
            // set breakpoint here and check the uri
            string uri = request.UriTemplateMatch.RequestUri.ToString();
            var tempServiceRequesting = ServiceRequesting;
            if (tempServiceRequesting != null) tempServiceRequesting(this, new ServiceRequestEventArgs(uri, method));
        }

        private void _enqueuing(string methodName, string module, NameValueCollection bodyParameters )
        {
            //Todo: refactor, DRY! See method above...
            logger.Info("-> {0}()", methodName);
            var request = WebOperationContext.Current.IncomingRequest;
            string method = request.Method;
            string uri = request.UriTemplateMatch.RequestUri.ToString();
            var tempEnqueuing = Enqueuing;
            if (tempEnqueuing != null) tempEnqueuing(this, new EnqueuingEventArgs(uri, method, module, bodyParameters));
        }

        public event EventHandler<ServiceRequestEventArgs> ServiceRequesting;
        public event EventHandler<EnqueuingEventArgs> Enqueuing;

    }





}
