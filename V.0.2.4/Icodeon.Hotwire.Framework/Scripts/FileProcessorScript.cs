using System;
using System.IO;
using System.Linq;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Modules;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Serialization;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Scripts
{
    public class FileProcessorScript : IScript
    {
        protected IHttpClientProvider _httpClient;
        protected readonly ProcessFileCallerBase _processFileCaller;
        protected readonly HotwireFilesProvider _fileprovider;
        protected readonly LoggerBase _logger;
        protected bool _isRunning = false;

        public bool isRunning { get { return _isRunning;  } }

        public string ScriptName
        {
            get { return "File Processor script."; }
        }

        public FileProcessorScript(HotwireFilesProvider fileprovider, LoggerBase logger, IHttpClientProvider httpClient, ProcessFileCallerBase processFileCaller)
        {
            _fileprovider = fileprovider;
            _logger = logger;
            _httpClient = httpClient;
            _processFileCaller = processFileCaller;
        }

        public virtual void Run(HotLogger logger, Utils.IConsoleWriter console, string folderPath)
        {
            // ignore the folder for now, because the exact folder used for downloading is currently configured in hotwireFilesProvider.
            // will change this later.
            Run(logger,console,null);
        }

        public virtual void Run(HotLogger logger, Utils.IConsoleWriter console)
        {
            try
            {
                _isRunning = true;

                EnqueueRequestDTO dto;

                while ((dto = GetNextImportFileToProcessMoveToProcessingOrDefault(console)) != null)
                {
                    ProcessFile(console, dto, logger, _httpClient,_processFileCaller);
                }
                console.WriteLine("Nothing left to process, exiting.");
            }
            finally
            {
                _isRunning = false;
            }
        }



        protected void ProcessFile(IConsoleWriter console, EnqueueRequestDTO dto, LoggerBase logger, IHttpClientProvider client, ProcessFileCallerBase processFileCaller)
        {
            console.WriteLine("Processing {0}",dto.ResourceFile);
            try
            {
                processFileCaller.CallProcessFileWaitForComplete(dto.GetTrackingNumber());
                _fileprovider.MoveFileAndSettingsFileToFolder(dto.GetTrackingNumber(), _fileprovider.ProcessingFolderPath, _fileprovider.ProcessedFolderPath);
            }
            catch (Exception ex)
            {
                //TODO: update IConsoleWriter to support writing erorrs! line below ignores fact that we have console writer injected above!
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

            EnqueueRequestDTO importFile = ReadImportFileAndMoveFromProcessQueueToProcessingFolder(importFileNamePath);
            return importFile;
        }

        protected EnqueueRequestDTO ReadImportFileAndMoveFromProcessQueueToProcessingFolder(string importFileNamePath)
        {
            string json = File.ReadAllText(importFileNamePath);
            EnqueueRequestDTO importFile = JSONHelper.Deserialize<EnqueueRequestDTO>(json);
            _fileprovider.MoveFileAndSettingsFileToFolder(importFile.GetTrackingNumber(),srcFolderPath: _fileprovider.ProcessQueueFolderPath,destFolderPath: _fileprovider.ProcessingFolderPath);
            return importFile;
        }


    }
}
