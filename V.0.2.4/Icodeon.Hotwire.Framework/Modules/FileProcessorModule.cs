using System;
using System.Collections.Generic;
using System.Net;
using Icodeon.Hotwire.Contracts;
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
    // rename file to FileProcessorHelper, this isnt a module anymore!

    public static class FileProcessorModule 
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static EnqueueRequestDTO ProcessFile(string trackingNumber, QueueDal dal)
        {
            // boundvariables("TRACKING-NUMBER")
            
            _logger.Trace("ProcessFile(trackingNumber:='{0}')",trackingNumber);
            var fileProvider = HotwireFilesProvider.GetFilesProviderInstance();
            // ADH: queue dal should be injected
            //var dal = new QueueDal(fileProvider);
            EnqueueRequestDTO dto = dal.GetByTrackingNumber(trackingNumber);
            var processor = new ProviderFactory().CreateFileProcessor();
            var parameters = dto.ToUnderScoreIcodeonCCPNamedNameValueCollectionPlusExtraHotwireParamsAndAnyExtraParamsPostedByClient();
            _logger.TraceParameters(parameters);
            _logger.Trace("process the file ...");
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
        }

        public const string ActionProcessFile = "PROCESS-FILE";

    } 
} 