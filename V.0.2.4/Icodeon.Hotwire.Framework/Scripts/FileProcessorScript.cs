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

        protected bool _isRunning = false;

        public event EventHandler<ExceptionEventArgs> ProcessException;

        public bool isRunning { get { return _isRunning;  } }

        public string ScriptName
        {
            get { return "File Processor script."; }
        }

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public FileProcessorScript(HotwireFilesProvider fileprovider, ProcessFileCallerBase processFileCaller)
        {
            _fileprovider = fileprovider;
            _processFileCaller = processFileCaller;
        }

        private void RaiseProcessException(Exception ex, EnqueueRequestDTO dto)
        {
            var tempHandler = ProcessException;
            if (tempHandler != null)
            {
                tempHandler(this, new ExceptionEventArgs(ex, ePipeLineSection.FileProcess, dto, null));
            }
        }



        public virtual void Run(Utils.IConsoleWriter console, string folderPath)
        {
            // ignore the folder for now, because the exact folder used for downloading is currently configured in hotwireFilesProvider.
            // will change this later.
            Run(console,null);
        }

        public virtual void Run(Utils.IConsoleWriter console)
        {
            EnqueueRequestDTO dto = null;
            try
            {
                _isRunning = true;

               

                while ((dto = GetNextImportFileToProcessMoveToProcessingOrDefault(console)) != null)
                {
                    ProcessFile(console, dto, _processFileCaller);
                }
                console.WriteLine("Nothing left to process, exiting.");
            }
            catch(Exception ex)
            {
                RaiseProcessException(ex,dto);                    
            }
            finally
            {
                _isRunning = false;
            }
        }



        protected void ProcessFile(IConsoleWriter console, EnqueueRequestDTO dto, ProcessFileCallerBase processFileCaller)
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
                RaiseProcessException(ex,dto);
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
