using System;
using System.Collections.Generic;
using System.Net;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Repository;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Modules
{
    public class FileProcessorModule : ModuleBase
    {

        protected override string ConfigurationSectionName
        {
            get { return Constants.Configuration.ProcessFileSectionName; }
        }

        public override object ProcessRequest(ParsedContext context)
        {
            DebugContract.Ensure(
                                 ()=> context.Match != null,
                                 () => context.ModuleConfig != null,
                                 () => context.MediaInfo != null,
                                 ()=> context.Logger != null);
            
            var logger = context.Logger;
            logger.Trace("ProcessRequest, {0}, {1}", context.ModuleConfig.Action, context.Url);

            IModuleEndpoint moduleConfig = context.ModuleConfig;
            switch (moduleConfig.Action)
            {

                case ActionProcessFile:
                    logger.Trace("SmokeTestModule -> PROCESS-FILE");
                    string trackingNumber = context.Match.BoundVariables["TRACKING-NUMBER"];
                    logger.Trace("Tracking Number = " + trackingNumber);
                    logger.Trace("Security for endpoint is " + moduleConfig.Security);
                    if (moduleConfig.Security==SecurityType.localonly)
                    {
                        if (!context.Url.IsLoopback)
                            throw new HttpModuleException(logger, HttpStatusCode.Forbidden, "Remote connections not allowed.");
                    }
                    var fileProvider = HotwireFilesProvider.GetFilesProviderInstance(logger);
                    // ADH: queue dal should be injected
                    var dal = new QueueDal(fileProvider, logger);
                    EnqueueRequestDTO dto = dal.GetByTrackingNumber(trackingNumber);
                    var processor = new ProviderFactory().CreateFileProcessor();
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
                        //RAISE EXCEPTION
                        throw;
                    }
                    return dto;

                case ActionEcho:
                    string message = context.Match.BoundVariables["SAY"];
                    return message;

                default: throw new HttpModuleException(HttpStatusCode.BadRequest, moduleConfig.Action + " is not a supported action.");
            }
        }

        public const string ActionProcessFile = "PROCESS-FILE";


        public const string SlashEncodingToken = "~";

        public static string EscapeSlashes(string src)
        {
            
            // deviation (by design for security reasons) in .NET 4 Uri class from the RFC spec
            // which means you cant (easily) pass slash as an uri parameter, 
            // which is the reason for the tokenising of the slash here. 
            // see: http://stackoverflow.com/questions/591694/url-encoded-slash-in-url for more info.
            return src.Replace("/", SlashEncodingToken);
        }

        public static string UnescapeSlashes(string src)
        {
            return src.Replace(SlashEncodingToken, "/");
        }


        public override IEnumerable<string> ActionNames
        {
            get { return new[] { ActionVersion, ActionProcessFile }; }
        }





    } 
} 