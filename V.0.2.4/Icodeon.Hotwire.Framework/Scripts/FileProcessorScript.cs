using System;
using System.IO;
using System.Linq;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Serialization;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Scripts
{
    public class FileProcessorScript : IScript
    {
        private readonly HotwireFilesProvider _fileprovider;
        private readonly Logger _logger;
        private bool _isRunning = false;

        public bool isRunning { get { return _isRunning;  } }

        public string ScriptName
        {
            get { return "File Processor script."; }
        }

        public FileProcessorScript(HotwireFilesProvider fileprovider, Logger logger)
        {
            _fileprovider = fileprovider;
            _logger = logger;
        }

        public void Run(LoggerBase logger, Utils.IConsoleWriter console, string folderPath)
        {
            // ignore the folder for now, because the exact folder used for downloading is currently configured in hotwireFilesProvider.
            // will change this later.
            Run(logger,console,null);
        }

        public void Run(LoggerBase logger, Utils.IConsoleWriter console)
        {
            try
            {
                _isRunning = true;

                EnqueueRequestDTO dto;
                while ((dto = GetNextImportFileToProcessMoveToProcessingOrDefault(console)) != null)
                {
                    ProcessFile(console, dto, logger);
                }
                console.WriteLine("Nothing left to process, exiting.");
            }
            finally
            {
                _isRunning = false;
            }
        }



        private void ProcessFile(IConsoleWriter console, EnqueueRequestDTO dto, LoggerBase logger)
        {
            console.WriteLine("Processing {0}",dto.ResourceFile);
            try
            {
                // process the file, wait for response, get endpoint by name = fileprocess
                string endpoint = ProcessFileSection.ReadConfig().GetEndpoint(dto.GetTrackingNumber());
                var uri = new Uri(endpoint);
                var client = new ProviderFactory(logger).CreateHttpClient();
                // move the import file from processing to processed folder 
                client.GetAndEnsureStatusIsSuccessful(uri);
                _fileprovider.MoveFileAndSettingsFileToFolder(dto.GetTrackingNumber(), _fileprovider.ProcessingFolderPath, _fileprovider.ProcessedFolderPath);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Error processing "+ dto.ResourceFile);
                _fileprovider.MoveFileAndSettingsFileFromProcessingFolderToErrorFolderWriteExceptionFile(dto.GetTrackingNumber(), ex);
            }
        }

        public EnqueueRequestDTO GetNextImportFileToProcessMoveToProcessingOrDefault(IConsoleWriter console)
        {
            _fileprovider.RefreshFiles();
            string importFileNamePath;
            string importFileName;
            do
            {
                importFileNamePath = _fileprovider.ProcessQueueFilePaths.SortByDateAscending().FirstOrDefault();
                if (importFileNamePath == null)
                {
                    _logger.Trace("nextFile==null, returning.");
                    return null;
                }
                importFileName = Path.GetFileName(importFileNamePath);
                var status = _fileprovider.GetStatusByImportFileName(Path.GetFileName(importFileNamePath));
                // must only have 1 status of queued for processing or error, otherwise pick the next file
                if (status != QueueStatus.QueuedForProcessing)
                {
                    var msg = string.Format("Skipping {0}, \tstatus is {1}.", importFileName, status);
                    console.WriteLine(msg);
                    _logger.Trace(msg);
                    // rename as skipped for now.
                    string destination = Path.Combine(_fileprovider.ProcessErrorFolderPath, importFileName + "." + status + EnqueueRequestDTO.SkippedExtension);
                    _logger.Trace("Renaming and moving to " + destination);
                    File.Move(importFileNamePath, destination);
                    importFileNamePath = null;
                    _fileprovider.RefreshFiles();
                }
            } while (importFileNamePath == null);

            string json = File.ReadAllText(importFileNamePath);
            EnqueueRequestDTO importFile = JSONHelper.Deserialize<EnqueueRequestDTO>(json);
            _fileprovider.MoveFileAndSettingsFileToFolder(importFile.GetTrackingNumber(),_fileprovider.ProcessQueueFolderPath,_fileprovider.ProcessingFolderPath);
            return importFile;
        }


    }
}
