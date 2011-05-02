using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.MediaTypes;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Modules
{
    public abstract class ModuleBase : IHttpModule
    {
        public void Dispose() { }

        // todo : Add in abstract DescriptionDTO GetDescription();
        abstract protected string ConfigurationSectionName { get; }
        public abstract IEnumerable<string> ActionNames { get; }

        protected abstract object ProcessRequest(HttpApplicationState applicationState,Stream inputStream,Uri url, UriTemplateMatch match, IModuleEndpoint config, IMediaInfo mediaInfo, IMapPath mapper, LoggerBase logger);
        
        //ADH: override this and return null in testing if you don't need logging.
        protected virtual LoggerBase GetLogger()
        {
            var logger = LogManager.GetLogger(ConfigurationSectionName);
            return new HotLogger(logger);
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += delegate
                                        {
                                            Uri url = context.Request.Url;
                                            HttpRequest request = context.Request;
                                            IModuleConfiguration configuration = new ModuleConfigurationCache(ConfigurationSectionName, context.Application).Configuration;
                                            if (configuration == null) throw new Exception("could not load configuration for httpModule:" + ConfigurationSectionName);
                                            if (!configuration.Active) return;
                                            if (configuration.MethodValidation==MethodValidation.beforeUriValidation)
                                            {
                                                if (!configuration.Endpoints.Any(ep1 => ep1.HttpMethods.Any(ep2 => ep2.Contains(request.HttpMethod)))) return;    
                                            }
                                            
                                            string rootUriString = string.Format("http://{0}:{1}/{2}", url.Host, url.Port,configuration.RootServiceName);
                                            var rootUri = new Uri(rootUriString);

                                            var endpoint = configuration.Endpoints.FirstOrDefault(ep => ep.Active && ep.UriTemplate.Match(rootUri, url) != null);
                                            if (endpoint==null) return;

                                            // needs unit test to prove this works correctly!
                                            if (configuration.MethodValidation == MethodValidation.afterUriValidation)
                                            {
                                                if (!endpoint.HttpMethods.Any(ep2 => ep2.Contains(request.HttpMethod)))
                                                {
                                                    context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                                                    context.Response.Write("method '"+ request.HttpMethod + "', not allowed.");
                                                    context.CompleteRequest();
                                                    return;
                                                }        
                                            }
                                            

                                            // WE HAVE ONE WINNER, A GENUINE REQUEST THAT PASSES ALL OUR FILTER TESTS, GO FORTH AND PROCESS IT FOLKS!
                                            // ====================================================================================================
                                            UriTemplateMatch match = endpoint.UriTemplate.Match(rootUri, url);

                                            LoggerBase logger = GetLogger();
                                            logger.Trace("");
                                            logger.Trace("Module: {0} v {1}", GetType(), AssemblyHelper.FrameworkVersion);
                                            logger.Trace("*****************************************************************");
                                            logger.Trace("{0} requested [{1}] -> \"{1}\"", request.UserHostAddress, request.HttpMethod, request.RawUrl);
                                            var responseWriter = new ResponsableHttpContextWriter(context.Response);
                                            
                                            logger.Trace("Reading endpoint configuration.");
                                            var mediaInfo = new MediaTypeFactory()[endpoint.MediaType];
                                            logger.Trace("Media type for configured endpoint is '{0}'.", mediaInfo.Type);

                                            try
                                            {
                                                logger.Trace("ProcessRequest(contextWriter, logger)", request.RawUrl);
                                                logger.Trace("creating HttpApplicationWrapper");
                                                var pathMapper = new HttpApplicationWrapper(context);
                                                logger.Trace("check that the configured service matches the registered service names");
                                                if(!ActionNames.Contains(endpoint.Action)) throw new ArgumentException(string.Format("Invalid action name ['{0}'] in the configuration. Must be one of:{1}",endpoint.Action,  String.Join(",",ActionNames)));

                                                //TODO: add in automatic oauth authentication, with a default provider so that secret can be easily looked up 
                                                //if(endpoint.Security==SecurityType.oauth)
                                                //{
                                                //    TODO:implement
                                                //}
                                                
                                                object result = ProcessRequest(context.Application, context.Request.InputStream, request.Url, match, endpoint, mediaInfo, pathMapper, logger);

                                                logger.Trace("Writing response.");
                                                //TODO: when I change process to take a Hotwire process context object, should provide a way to set the response code!
                                                ContextHelper.WriteMediaResponse(responseWriter, mediaInfo,result, HttpStatusCode.OK, logger);
                                            }
                                            catch(HttpModuleException mex)
                                            {
                                                logger.Error("({0}) {1}", mex.StatusCode, mex);
                                                context.Response.ContentType = mediaInfo.ContentType;
                                                context.Response.StatusCode = (int) mex.StatusCode;
                                                // cant set any custom text with this?
                                                // this is by design for most of the core status codes ?? to stop users from trying to use them for something else
                                                // which would literally "break the internet" ;-p Makes sense!
                                            }

                                            catch (Exception ex)
                                            {
                                                logger.FatalException(ex.Message, ex);
                                                logger.Trace(ex.ToString());
                                                var hotException = new HotwireExceptionDTO(ex);
                                                // if debug
                                                ContextHelper.WriteMediaResponse(responseWriter,mediaInfo,hotException,HttpStatusCode.InternalServerError,logger);
                                            }
                                            finally
                                            {
                                                // call complete request to avoid any of the customer's handlers or 
                                                // pipeline from processing the hotwire module 
                                                // complete request is a bit confusing as it sounds like you're asking
                                                // IIS to carry on and complete the request and not STOP processing.
                                                context.CompleteRequest();
                                            }
                                        };
        }


    }
}