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
        protected virtual HotLogger GetLogger()
        {
            var logger = LogManager.GetLogger(ConfigurationSectionName);
            return new HotLogger(logger);
        }

        // Internal so that IsConfigurationActiveAndRequestAMatchForAnyConfiguredEndpoints can be unit tested.
        internal class EndpointMatchResult
        {
            public IModuleEndpoint Endpoint { get; set; }
            public UriTemplateMatch Match { get; set; }
        }
         
        /// <remarks>Internal so that this can be unit tested</remarks>
        /// <returns>First endpoint that matches the request rules, else null.</returns>
        internal EndpointMatchResult IsConfigurationActiveAndRequestAMatchForAnyConfiguredEndpoints(IModuleConfiguration configuration, string httpMethod, Uri requestUrl)
        {
            // NB! Remember to optimise for speed as this get's called for every single request on clients server. It's potentially a website killer!
            if ((configuration == null) || (!configuration.Active)) return null;
            if (configuration.MethodValidation == MethodValidation.beforeUriValidation)
            {
                if (!configuration.Endpoints.Any(ep1 => ep1.HttpMethods.Any(ep2 => ep2.Contains(httpMethod)))) return null;
            }

            string rootUriString = string.Format("http://{0}:{1}/{2}", requestUrl.Host, requestUrl.Port, configuration.RootServiceName);
            var rootUri = new Uri(rootUriString);
            var endpoint = configuration.Endpoints.FirstOrDefault(ep => ep.Active && ep.UriTemplate.Match(rootUri, requestUrl) != null);
            if (endpoint == null) return null;
            
            if (configuration.MethodValidation == MethodValidation.afterUriValidation)
            {
                if (!endpoint.HttpMethods.Any(ep2 => ep2.Contains(httpMethod)))
                {
                    throw new HttpModuleException(HttpStatusCode.MethodNotAllowed, "method '" + httpMethod + "', not allowed.");
                }
            }

            return new EndpointMatchResult
                       {
                           Endpoint = endpoint,
                           Match = endpoint.UriTemplate.Match(rootUri, requestUrl)
                       };
        }

        // TODO: Rename when this is proven to work and is testable!
        private void TestableProcessRequest(Uri requestUrl, IModuleConfiguration configuration, string httpMethod, string userHostAddress, IHttpResponsableWriter responseWriter, IMapPath pathMapper, HttpApplicationState applicationState, Stream inputStream, EndpointMatchResult endpointMatch)
        {
            // step 1, test ModuleBase, forget about derived classes
            // step 2, once you KNOW that all the functionality of the base class has been tested, then with derived classes
            //          you can simply test THEIR behavior, ignoring the behavior of the base class which is already tested?

            string rootUriString = string.Format("http://{0}:{1}/{2}", requestUrl.Host, requestUrl.Port, configuration.RootServiceName);
            var rootUri = new Uri(rootUriString);
            

            LoggerBase logger = GetLogger();
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

            //TODO: add in automatic oauth authentication, with a default provider so that secret can be easily looked up 
            //if(endpoint.Security==SecurityType.oauth)
            //{
            //    TODO:implement
            //}

            object result = ProcessRequest(applicationState, inputStream, requestUrl, endpointMatch.Match, endpoint, mediaInfo, pathMapper, logger);

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
                //TODO: replace applicationState with IAppCache below ?
                IModuleConfiguration configuration = new ModuleConfigurationCache(ConfigurationSectionName, applicationState).Configuration;
                var endpointMatch = IsConfigurationActiveAndRequestAMatchForAnyConfiguredEndpoints(configuration, httpMethod, request.Url);
                if (endpointMatch == null) return;
                                            
                HotLogger logger = GetLogger();
                var response = context.Response;
                IHttpResponsableWriter writer = new ResponsableHttpContextWriter(response);

                try
                {
                    string userHostAddress = request.UserHostAddress;
                    IMapPath pathMapper = new HttpApplicationWrapper(context);
                    Stream inputStream = request.InputStream;
                    //TODO: replace applicationState with IAppCache below ?
                    TestableProcessRequest(request.Url, configuration, httpMethod,userHostAddress, writer, pathMapper, applicationState,inputStream, endpointMatch);
                }
                catch (HttpModuleException mex)
                {
                    logger.Error("({0}) {1}", mex.StatusCode, mex);
                    var mediaInfo = new MediaTypeFactory()[endpointMatch.Endpoint.MediaType];
                    context.Response.ContentType = mediaInfo.ContentType;
                    context.Response.StatusCode = (int)mex.StatusCode;
                    // cant set any custom text with this?
                    // this is by design for most of the core status codes ?? to stop users from trying to use them for something else
                    // which would literally "break the internet"? ;-p Makes sense, I  guess.
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