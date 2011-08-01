using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.MediaTypes;
using Icodeon.Hotwire.Framework.Security;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Modules
{
    public abstract class ModuleBase : IHttpModule
    {
        public void Dispose() { }

        abstract protected string ConfigurationSectionName { get; }
        public abstract IEnumerable<string> ActionNames { get; }
        
        protected abstract object ProcessRequest(HttpApplicationState applicationState,NameValueCollection queueParameters,Uri url, UriTemplateMatch match, IModuleEndpoint moduleConfig, IMediaInfo mediaInfo, IMapPath mapper, LoggerBase logger);
        
        //ADH: override this and return null in testing if you don't need logging.
        protected virtual HotLogger GetLogger()
        {
            var logger = LogManager.GetLogger(ConfigurationSectionName);
            return new HotLogger(logger);
        }

        protected virtual IAuthenticateRequest GetRequestAuthenticator(HotLogger logger, SecurityType endpointAuthorisation)
        {
            switch (endpointAuthorisation)
            {
                case SecurityType.none:
                    return new NoSecurityRequestAuthenticator();
                case SecurityType.oauth:
                    return new OAuthRequestAuthenticator(logger);
                case SecurityType.localonly:
                    return new LocalOnlyRequestAuthenticator();
                default:
                    throw new ArgumentOutOfRangeException("endpointAuthorisation");
            }
        }

        internal void PrepareAndProcessRequest(Uri requestUrl, IModuleConfiguration configuration, string httpMethod, string userHostAddress, IHttpResponsableWriter responseWriter, IMapPath pathMapper, HttpApplicationState applicationState, Stream inputStream, EndpointMatch endpointMatch)
        {
            // step 1, test ModuleBase, forget about derived classes
            // step 2, once you KNOW that all the functionality of the base class has been tested, then with derived classes
            //          you can simply test THEIR behavior, ignoring the behavior of the base class which is already tested?

            HotLogger logger = GetLogger();
            logger.Trace("");
            logger.Trace("Module: {0} v {1}", GetType(), AssemblyHelper.FrameworkVersion);
            logger.Trace("*****************************************************************");
            logger.Trace("{0} requested [{1}] -> \"{1}\"", userHostAddress, httpMethod, requestUrl);

            logger.Trace("Reading endpoint configuration.");
            
            IModuleEndpoint endpoint = endpointMatch.Endpoint;
            var mediaInfo = new MediaTypeFactory()[endpoint.MediaType];
            logger.Trace("Media type for configured endpoint is '{0}'.", mediaInfo.Type);

            logger.Trace("ProcessRequest(contextWriter, logger)", requestUrl);
            logger.Trace("creating HttpApplicationWrapper");

            logger.Trace("check that the configured service matches the registered service names");
            if (!ActionNames.Contains(endpoint.Action)) throw new ArgumentException(string.Format("Invalid action name ['{0}'] in the configuration. Must be one of:{1}", endpoint.Action, String.Join(",", ActionNames)));

            // NB! there is currently no authorisation ('What' permisssions) component, ONLY AUTHENTICATION (who?) as a result it may be tempting to simply write if (endpoint.Security!=none) below
            // but that assumes that if you do nothing then you have access to everything, which is brittle and not scalable, by default you should have access to nothing
            // and a seperate step of authorisation provides permission to resources, which is why there is a placeholder NoSecurityRequestAuthenticator being 
            // provided. (Worth reviewing and neatening up later, will make more sense if applied to a proper role based example.)
            // -------------------------------------------------------------------------------------------------------------------
            var queueParameters = inputStream.ParseNameValues();
            var authenticator = GetRequestAuthenticator(logger, endpoint.Security);
            authenticator.AuthenticateRequest(queueParameters);
            object result = ProcessRequest(applicationState, queueParameters, requestUrl, endpointMatch.Match, endpoint, mediaInfo, pathMapper, logger);

            logger.Trace("Writing response.");
            
            //TODO: when I change process to take a Hotwire process context object, should provide a way to set the response code!
            ContextHelper.WriteMediaResponse(responseWriter, mediaInfo, result, HttpStatusCode.OK, logger);
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += delegate
            {
                var request = context.Request;
                HttpApplicationState applicationState = context.Application;
                string httpMethod = request.HttpMethod;
                IAppCache appCache = new AppCacheWrapper(context.Application);
                IModuleConfiguration configuration = new ModuleConfigurationCache(ConfigurationSectionName, appCache).Configuration;
                var matcher = new EndpointRequestMatcher(configuration);
                var endpointMatch = matcher.MatchRequestOrNull(httpMethod, request.Url);
                if (endpointMatch == null) return;
                                            
                HotLogger logger = GetLogger();
                var response = context.Response;
                IHttpResponsableWriter writer = new ResponsableHttpContextWriter(response);

                try
                {
                    string userHostAddress = request.UserHostAddress;
                    IMapPath pathMapper = new HttpApplicationWrapper(context);
                    Stream inputStream = request.InputStream;
                    PrepareAndProcessRequest(request.Url, configuration, httpMethod,userHostAddress, writer, pathMapper, applicationState,inputStream, endpointMatch);
                }
                catch (HttpModuleException mex)
                {
                    logger.Error("({0}) {1}", mex.StatusCode, mex);
                    var mediaInfo = new MediaTypeFactory()[endpointMatch.Endpoint.MediaType];
                    context.Response.ContentType = mediaInfo.ContentType;
                    context.Response.StatusCode = (int)mex.StatusCode;
                    // cant set any custom text with this?
                    // this is by design for most of the core status codes ?? to stop users from trying to use them for something else
                    // which would literally "break the internet"? i.e. we need people to trust the codes, not the description. ;-p Makes sense, I  guess.
                }
                catch (Exception ex)
                {
                    logger.FatalException(ex.Message, ex);
                    logger.Trace(ex.ToString());
                    var hotException = new HotwireExceptionDTO(ex);
                    var mediaInfo = new MediaTypeFactory()[endpointMatch.Endpoint.MediaType];
                    ContextHelper.WriteMediaResponse(writer, mediaInfo, hotException, HttpStatusCode.InternalServerError, logger);
                }
                finally
                {
                    context.CompleteRequest();
                }

            };
        }




    }
}