using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.DTOs;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Repository;
using Icodeon.Hotwire.Framework.Serialization;
using Icodeon.Hotwire.Framework.SmokeTests;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Modules
{
    //TODO: Convert to use ModuleBase!

    public class SmokeTestModule : ModuleBase
    {

        protected override string ConfigurationSectionName
        {
            get { return Constants.Configuration.SmokeTestSectionName; }
        }

        protected override object ProcessRequest(HttpApplicationState applicationState, Stream inputStream, Uri url, UriTemplateMatch match, IModuleEndpoint config, IMediaInfo mediaInfo, IMapPath mapper, Logger logger)
        {
            Contract.Requires(applicationState!=null);
            Contract.Requires(match != null);
            Contract.Requires(config != null);
            Contract.Requires(mediaInfo != null);
            Contract.Requires(mapper != null);
            Contract.Requires(logger != null);
            
            logger.LogMethodCall("ProcessRequest", applicationState, inputStream, url, match, config, mediaInfo, mapper,logger);

            switch (config.Action)
            {
                case ActionProcessFile:
                    logger.Trace("SmokeTestModule -> PROCESS-FILE");
                    string trackingNumber = match.BoundVariables["TRACKING-NUMBER"];
                    logger.Trace("Tracking Number = " + trackingNumber);
                    logger.Trace("Security for endpoint is " + config.Security);
                    if (config.Security==SecurityType.localonly)
                    {
                        if (!url.IsLoopback)
                            throw new HttpModuleException(logger, HttpStatusCode.Forbidden, "Remote connections not allowed.");
                    }
                    var fileProvider = HotwireFilesProvider.GetFilesProviderInstance(logger);
                    var dal = new QueueDal(fileProvider, logger);
                    EnqueueRequestDTO dto = dal.GetByTrackingNumber(trackingNumber);
                    var processor = new ProviderFactory(logger).CreateFileProcessor();
                    var parameters = dto.ToUnderScoreIcodeonCCPNamedNameValueCollectionPlusExtraHotwireParamsAndAnyExtraParamsPostedByClient();
                    logger.TraceParameters(parameters);
                    logger.Trace("process the file ...");
                    try
                    {
                        processor.ProcessFile(dto.GetTrackingNumber(), dto.TransactionId, parameters);
                        // all we need to do is move the file as the last step, i.e. change the status and save. Needs to be transactional
                        // for now we'll leave that up to the caller to do.
                    }
                    catch (Exception ex)
                    {
                        fileProvider.MoveFileAndSettingsFileFromProcessingFolderToErrorFolderWriteExceptionFile(dto.ResourceFile,ex);
                        throw;
                    }
                    return dto;

                case ActionDebug:
                    throw new NotImplementedException();

                case ActionVersion:
                    return AssemblyHelper.FrameworkVersion;

                case ActionEcho:
                    string message = match.BoundVariables["SAY"];
                    return message;

                case ActionFileCopyTest:
                    // TODO: move the folders below to config!
                    var srcFolderPathcopy = mapper.MapPath("~/App_Data/TestFolder");
                    var destFolderPathcopy = mapper.MapPath("~/App_Data/HotwireFolders");
                    TestCreateAndMoveFile(srcFolderPathcopy, destFolderPathcopy,true);
                    // can datacontract serializer return this as JSON/XML etc?
                    // may have to return a fixed DTO
                    return new
                               {
                                    Source = srcFolderPathcopy,
                                    Destionation = destFolderPathcopy
                               };

                case ActionBackgroundThreadMoveTest:
                    var srcFolderPath = mapper.MapPath("~/App_Data/TestFolder/Src");
                    var destFolderPath = mapper.MapPath("~/App_Data/TestFolder/Dest");
                    var threadStart = new ThreadStart(() => TestCreateAndMoveFile(srcFolderPath, destFolderPath, false));
                    var testThread = new Thread(threadStart);
                    testThread.Start();
                    // could wait for thread to finish (join) then do a test to see if it really copied?
                    return "Hotwire: Background Thread Copy Test. Please check files manually";


                case ActionUriTemplateSet:
                    // TODO: encapsulate into new class that handles all this that can change uritemplate for any endpoint
                    var moduleConfigurationCache = new ModuleConfigurationCache(Constants.Configuration.QueuesSectionName, applicationState);
                    const string uritemplatePathVariable = "URITEMPLATE";
                    // change the uri template for enqueue request
                    if (!match.BoundVariables.AllKeys.Contains(uritemplatePathVariable)) throw new HttpModuleException(HttpStatusCode.BadRequest,"Could not find uri template path variable:" + uritemplatePathVariable);
                    // need forwardslashes in uriTemplates, but can't pass them as uri parameters because of a bug in a Microsoft Uri class, eurgh!
                    string newUriTemplate = UnescapeSlashes(match.BoundVariables[uritemplatePathVariable]);
                    logger.Trace("new newUriTemplate=" + newUriTemplate);
                    logger.Trace("reading queueuConfig");
                    var queueConfig = moduleConfigurationCache.RefreshConfiguration();
                    logger.Trace("finding custom endpoint");
                    var endpoint = queueConfig.Endpoints.FirstOrDefault(e => e.Name.Equals("custom", StringComparison.OrdinalIgnoreCase));
                    if (endpoint == null) throw new ArgumentNullException("could not find custom endpoint in the queue config, with name='q1'");
                    logger.Trace("Found endpoint:" + endpoint.ToString());
                    logger.Trace("setting new uriTemplate and setting active");
                    endpoint.Active = true;
                    endpoint.UriTemplate = new UriTemplate(newUriTemplate);
                    logger.Trace("caching the configuration");
                    moduleConfigurationCache.Configuration = queueConfig;
                    return "UriTemplate set to:" + newUriTemplate.ToString();

                case ActionUriTemplateReset:
                    var moduleConfigurationCache2 = new ModuleConfigurationCache(Constants.Configuration.QueuesSectionName, applicationState);
                    logger.Trace("resetting uriTemplate");
                    moduleConfigurationCache2.RefreshConfiguration();
                    return "UriTemplate reset.";

                default: throw new HttpModuleException(HttpStatusCode.BadRequest, config.Action + " is not a supported action.");
            }
        }

        // action names 
        public const string ActionUriTemplateSet = "URITEMPLATE-SET";
        public const string ActionUriTemplateReset = "URITEMPLATE-RESET";
        public const string ActionDebug = "DEBUG";
        public const string ActionEcho = "ECHO";
        public const string ActionVersion = "VERSION";
        public const string ActionFileCopyTest = "FILE-COPY-TEST";
        public const string ActionBackgroundThreadMoveTest = "BACKGROUND-THREAD-MOVE-TEST";
        public const string ActionProcessFile= "PROCESS-FILE";


        public const string slashEncodingToken = "~";

        public static string EscapeSlashes(string src)
        {
            
            // deviation (by design for security reasons) in .NET 4 Uri class from the RFC spec
            // which means you cant (easily) pass slash as an uri parameter, 
            // which is the reason for the tokenising of the slash here. 
            // see: http://stackoverflow.com/questions/591694/url-encoded-slash-in-url for more info.
            return src.Replace("/", slashEncodingToken);
        }

        public static string UnescapeSlashes(string src)
        {
            return src.Replace(slashEncodingToken, "/");
        }


        public override IEnumerable<string> ActionNames
        {
            get { return new[] { ActionUriTemplateReset, ActionUriTemplateSet, ActionVersion, ActionDebug, ActionEcho, ActionFileCopyTest, ActionBackgroundThreadMoveTest, ActionProcessFile }; }
        }


        private void TestCreateAndMoveFile(string sourceFolderPath, string destFolderPath, bool createFile)
        {
            if (!Directory.Exists(destFolderPath)) throw new DirectoryNotFoundException(destFolderPath);
            if (!Directory.Exists(sourceFolderPath)) throw new DirectoryNotFoundException(sourceFolderPath);

            string filename = createFile ? string.Format("{0}.txt", Guid.NewGuid()) : "moveme.txt";
            var src = Path.Combine(sourceFolderPath, filename);
            var dest = Path.Combine(destFolderPath, filename);
            if (createFile) File.WriteAllText(src, "hello world");
            File.Move(src, dest);
        }




    } 
} 