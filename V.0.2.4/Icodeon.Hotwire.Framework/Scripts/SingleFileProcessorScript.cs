using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Modules;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Scripts
{
    public class SingleFileProcessorScript : FileProcessorScript 
    {
        private readonly string _filenameContains;

        public SingleFileProcessorScript(
            string filenameContains,
            HotwireFilesProvider fileprovider, 
            LoggerBase logger, 
            IHttpClientProvider httpClient, 
            ProcessFileCallerBase processFileCaller) : base(fileprovider, logger, httpClient, processFileCaller)
        {
            _filenameContains = filenameContains;
        }

        public override void Run(HotLogger logger, IConsoleWriter console)
        {
            try
            {
                _isRunning = true;
                var dto = EnsureFileStatusIsQueuedReadImportAndMoveToProcessingFolder(console, _filenameContains);
                if (dto!=null) ProcessFile(console, dto, logger, _httpClient, _processFileCaller);
                console.WriteLine("finished processing single file, exiting.");
            }
            finally
            {
                _isRunning = false;
            }
        }

        public EnqueueRequestDTO EnsureFileStatusIsQueuedReadImportAndMoveToProcessingFolder(IConsoleWriter console, string filenameContains)
        {
            _logger.Trace("request to move specific file containing '" + filenameContains + "' to processingOrDefault.");
            var matches = _fileprovider.ProcessQueueFilePaths.Where(f => f.Contains(filenameContains));
            if (matches.Count()>1)
            {
                var msg = "More than 1 file matches filter '" + filenameContains + ". Must match a single file only in order to process. the following files match:";
                _logger.Trace(msg);
                Console.WriteLine(msg);
                matches.ToList().ForEach(Console.WriteLine);
                return null;
            }
            var importFileNameAndPath = _fileprovider.ProcessQueueFilePaths.FirstOrDefault(f => f.Contains(filenameContains));
            if (importFileNameAndPath == null)
            {
                var msg = "Could not find a file in the process queue containing '" + filenameContains + "'. No file processed.";
                _logger.Trace(msg);
                Console.WriteLine(msg);
                return null;
            }
            var importFileName = Path.GetFileName(importFileNameAndPath);
            var status = _fileprovider.GetStatusByImportFileName(importFileName);
            if (status != QueueStatus.QueuedForProcessing)
            {
                var msg = "Status of '" + importFileName + "' is " + status + ". Cannot process this file.";
                _logger.Trace(msg);
                Console.WriteLine(msg);
                return null;
            }
            return ReadImportFileAndMoveFromProcessQueueToProcessingFolder(importFileNameAndPath);
        }
    }
}
