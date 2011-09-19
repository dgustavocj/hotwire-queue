using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Serialization;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Repositories
{
    public class QueueDal
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly HotwireFilesProvider _fileModel;

        public QueueDal(HotwireFilesProvider fileModel)
        {
            _fileModel = fileModel;
        }

        private const string requestExtension = ".import";

        public void Save(EnqueueRequestDTO dto, QueueStatus status)
        {
            string folder = _fileModel.GetFolderByStatus(status);
            string enqueueFilename = dto.GetImportFileName();
            string enqueueFilePath = Path.Combine(folder, enqueueFilename);
            _logger.Trace("\tenqueueFilePath = {0}", enqueueFilePath);
            string json = JSONHelper.Serialize(dto);
            _logger.Trace("\tWriting enqueueRequest file.");
            File.WriteAllText(enqueueFilePath, json);
        }

        public EnqueueRequestDTO GetByTrackingNumber(string trackingNumber)
        {
            _logger.Trace("GetProcessingEnqueueRequest('{0}')",trackingNumber);
            string filePath = Path.Combine(_fileModel.ProcessingFolderPath, trackingNumber + requestExtension);
            if (!File.Exists(filePath))
            {
                throw new HttpModuleException(HttpStatusCode.NotFound, "Could not find enqueueRequest file '" + filePath + "'. Tracking number requested was '" + trackingNumber  +"'");
            }
            var json = File.ReadAllText(filePath);
            EnqueueRequestDTO dto = JSONHelper.Deserialize<EnqueueRequestDTO>(json);
            return dto;
        }
    }
}
