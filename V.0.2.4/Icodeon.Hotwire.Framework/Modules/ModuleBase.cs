using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Web;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.DTOs;
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

        public event EventHandler<ExceptionEventArgs> ProcessRequestException;

        public const string ActionVersion = "VERSION";
        public const string ActionEcho = "ECHO";
        public const string ActionIndex = "INDEX";

        public abstract object ProcessRequest(ParsedContext context);
        
        //ADH: override this and return null in testing if you don't need logging.
        protected virtual HotLogger GetLogger()
        {
            var logger = LogManager.GetLogger(ConfigurationSectionName);
            return new HotLogger(logger);
        }

        private IEnumerable<string> GetActionNamesIncludingDefaults()
        {
            foreach (var actionName in ActionNames)
            {
                yield return actionName;
            }
            yield return ActionIndex;
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

        internal void PrepareAndProcessRequest(StreamingContext context, EndpointMatch endpointMatch)
        {
            HotLogger logger = GetLogger();
            logger.Trace("");
            logger.Trace("Module: {0} v {1}", GetType(), AssemblyHelper.FrameworkVersion);
            logger.Trace("*****************************************************************");
            logger.Trace("{0} requested [{1}] -> \"{1}\"", context.UserHostAddress, context.HttpMethod,context.Url);

            logger.Trace("Reading endpoint configuration.");
            
            IModuleEndpoint endpoint = endpointMatch.Endpoint;
            var mediaInfo = new MediaTypeFactory()[endpoint.MediaType];
            logger.Trace("Media type for configured endpoint is '{0}'.", mediaInfo.Type);

            logger.Trace("ProcessRequest(contextWriter, logger)", context.Url);
            logger.Trace("creating HttpApplicationWrapper");

            logger.Trace("check that the configured service matches the registered service names");
            if (!GetActionNamesIncludingDefaults().Contains(endpoint.Action)) throw new ArgumentException(string.Format("Invalid action name ['{0}'] in the configuration. Must be one of:{1}", endpoint.Action, String.Join(",", GetActionNamesIncludingDefaults())));

            // NB! there is currently no authorisation ('What' permisssions) component, ONLY AUTHENTICATION (who?) as a result it may be tempting to simply write if (endpoint.Security!=none) below
            // but that assumes that if you do nothing then you have access to everything, which is brittle and not scalable, by default you should have access to nothing
            // and a seperate step of authorisation provides permission to resources, which is why there is a placeholder NoSecurityRequestAuthenticator being 
            // provided. (Worth reviewing and neatening up later, will make more sense if applied to a proper role based example.)
            // -------------------------------------------------------------------------------------------------------------------
            var requestParameters =context.InputStream.ParseNameValues();
            var authenticator = GetRequestAuthenticator(logger, endpoint.Security);
            authenticator.AuthenticateRequest(requestParameters);

            var parsedContext = new ParsedContext
            {
                AppCache = context.ApplicationCache,
                Logger = logger,
                Match = endpointMatch.Match,
                MediaInfo = mediaInfo,
                ModuleConfig = endpoint,
                PathMapper = context.PathMapper,
                RequestParameters = requestParameters,
                Url = context.Url
            };
            object result = ProcessRequest(parsedContext);

            logger.Trace("Writing response.");
            
            ContextHelper.WriteMediaResponse(context.HttpWriter, mediaInfo, result, HttpStatusCode.OK, logger);
        }

        [DataContract]
        public class ServiceListDTO
        {
            [DataMember]
            public string ModuleName { get; set; }

            [DataMember]
            public List<EndpointDTO> Endpoints { get; set; }

        }


        private void RaiseProcessException(Exception ex, HttpRequestContext request)
        {
            var tempHandler = ProcessRequestException;
            if (tempHandler != null)
            {
                tempHandler(this, new ExceptionEventArgs(ex, ePipeLineSection.HttpModule, null, request));
            }
        }


        public void BeginRequest(StreamingContext context)
        {
            var matcher = new EndpointRequestMatcher(context.Configuration);
            var endpointMatch = matcher.MatchRequestOrNull(context.HttpMethod, context.Url);
            if (endpointMatch==null) return;
            // NB! get the logger AFTER checking if this request matches the httpMethod and Uri, because getting a logger can be very slow,
            // and we don't want this called on every single http request on this server!
            if(endpointMatch.Endpoint.Action==ActionIndex)
            {
                var serviceList = new ServiceListDTO
                                      {
                                          ModuleName = GetType().ToString(),
                                          Endpoints = context.Configuration.Endpoints.Select(e=> e.ToDTO()).ToList()
                                      };
                ContextHelper.WriteMediaResponse(context.HttpWriter, new MediaTypeFactory()[endpointMatch.Endpoint.MediaType], serviceList, HttpStatusCode.OK, HotLogger.NullLogger);
                context.CompleteRequest();
                return;
            }
            if (endpointMatch.Endpoint.Action == ActionVersion)
            {
                string version = AssemblyHelper.FrameworkVersion;
                ContextHelper.WriteMediaResponse(context.HttpWriter, new MediaTypeFactory()[endpointMatch.Endpoint.MediaType], version, HttpStatusCode.OK, HotLogger.NullLogger);
                context.CompleteRequest();
                return;
            }

            if (endpointMatch.Endpoint.Action == ActionEcho)
            {
                string message = endpointMatch.Match.BoundVariables["SAY"];
                ContextHelper.WriteMediaResponse(context.HttpWriter, new MediaTypeFactory()[endpointMatch.Endpoint.MediaType], message, HttpStatusCode.OK, HotLogger.NullLogger);
                context.CompleteRequest();
                return;
            }
            var logger = context.GetLogger();
            try
            {
                PrepareAndProcessRequest(context,endpointMatch);
            }
            catch (HttpModuleException mex)
            {
                logger.Error("({0}) {1}", mex.StatusCode, mex);
                var mediaInfo = new MediaTypeFactory()[endpointMatch.Endpoint.MediaType];
                
                context.HttpWriter.ContentType = mediaInfo.ContentType;
                context.HttpWriter.StatusCode = (int)mex.StatusCode;
                // cant set any custom text with this?
                // this is by design for most of the core status codes ?? to stop users from trying to use them for something else
                // which would literally "break the internet"? i.e. we need people to trust the codes, not the description. ;-p Makes sense, I  guess.
                RaiseProcessException(mex, context);
            }
            catch (Exception ex)
            {
                logger.FatalException(ex.Message, ex);
                logger.Trace(ex.ToString());
                var mediaInfo = new MediaTypeFactory()[endpointMatch.Endpoint.MediaType];
                if(context.Configuration.Debug)
                {
                    var hotException = new HotwireExceptionDTO(ex);
                    ContextHelper.WriteMediaResponse(context.HttpWriter, mediaInfo, hotException, HttpStatusCode.InternalServerError, logger);
                }
                else
                {
                    var hotExceptionSummary = new HotwireExceptionSummaryDTO(ex);
                    ContextHelper.WriteMediaResponse(context.HttpWriter, mediaInfo, hotExceptionSummary, HttpStatusCode.InternalServerError, logger);
                }
                RaiseProcessException(ex, context);
            }
            finally
            {
                if (context.CompleteRequest!=null) context.CompleteRequest();
            }
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += delegate
            {
                IAppCache appCache = new AppCacheWrapper(context.Application);
                IModuleConfiguration configuration =
                    new ModuleConfigurationCache(ConfigurationSectionName, appCache).
                        Configuration;
                var writer = new ResponsableHttpContextWriter(context.Response);
                IMapPath pathMapper = new HttpApplicationWrapper(context);
                Stream inputStream = context.Request.InputStream;
                var hotContext = new StreamingContext
                                        {
                                            ApplicationCache = appCache,
                                            Configuration = configuration,
                                            HttpMethod = context.Request.HttpMethod,
                                            Url = context.Request.Url,
                                            GetLogger = GetLogger,
                                            HttpWriter = writer,
                                            UserHostAddress = context.Request.UserHostAddress,
                                            PathMapper = pathMapper,
                                            InputStream = inputStream,
                                            CompleteRequest = context.CompleteRequest
                                        };
                BeginRequest(hotContext);
            };
        }




    }
}