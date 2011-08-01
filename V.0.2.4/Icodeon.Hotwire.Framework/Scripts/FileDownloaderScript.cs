using System;
using System.IO;
using System.Linq;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Events;
using Icodeon.Hotwire.Framework.FolderWatcher;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Serialization;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

// ADH: To check, should this class be disposable / are there any unmanaged resources?

namespace Icodeon.Hotwire.Framework.Scripts
{
    public class FileDownloaderScript : IScript, IFolderWatcherScript
    {
        private readonly IDownloaderFilesProvider _downloaderFiles;

        private readonly IClientDownloader _clientDownloader;
        private bool _isRunning = false; 
        public bool isRunning { get { return _isRunning; } }

        public string ScriptName
        {
            get { return "File downloader script."; }
        }

        public event EventHandler<EnqueueRequestEventArgs> Downloading;
        public event EventHandler<FileInfoEventArgs> Downloaded;

        public FileDownloaderScript(IDownloaderFilesProvider downloaderFilesProvider,IClientDownloader clientDownloader)
        {
            _downloaderFiles = downloaderFilesProvider;
            _clientDownloader = clientDownloader;
        }


        public void Run(LoggerBase logger, IConsoleWriter console, string folderPath)
        {
            Run(logger,console,null);
        }

        public void Run(HotLogger logger, IConsoleWriter console)
        {
            try
            {
                _isRunning = true;
                EnqueueRequestDTO dto;
                while((dto = GetNextImportFileToDownloadMoveToDownloadingOrDefault(logger, console)) != null)
                {
                    DownloadFile(logger, console, dto);
                }
                console.WriteLine("Nothing left to download, exiting.");
            }
            finally
            {
                _isRunning = false;
            }
        }



        private void DownloadFile(HotLogger logger, IConsoleWriter console, EnqueueRequestDTO dto )
        {
            string trackingFilePath = Path.Combine(_downloaderFiles.DownloadingFolderPath, dto.GetTrackingNumber());
            File.WriteAllText(trackingFilePath, "");

            // raise downloading event
            var downloading = Downloading;
            if (downloading != null) downloading(this, new EnqueueRequestEventArgs(dto));
            console.Write("Downloading {0}",dto.ResourceFile);
            
            // Then the ext_resource_link file is downloaded and saved to the process Queue folder
            // And the .import file is moved from downloading to the processQueue folder
            // =============================================================================
            try
            {
                string destination = Path.Combine(_downloaderFiles.ProcessQueueFolderPath, dto.GetTrackingNumber());
                var downloadResult = _clientDownloader.DownloadFileWithTiming(logger, new Uri(dto.ExtResourceLinkContent),destination);
                console.WriteLine("\t{0:.00} KB in {1:.0} seconds, {2:.0,15} Kb/s", downloadResult.Kilobytes, downloadResult.Seconds, downloadResult.KbPerSec);
                // move the import file from downloading to process queue folder 
                string importSource = Path.Combine(_downloaderFiles.DownloadingFolderPath, dto.GetImportFileName());
                string importDest = Path.Combine(_downloaderFiles.ProcessQueueFolderPath, dto.GetImportFileName());
                // delete the zero byte file!
                File.Move(importSource,importDest);
                File.Delete(trackingFilePath);
                var downloaded = Downloaded;
                if (downloaded != null) downloaded(this, new FileInfoEventArgs(downloadResult.DownloadedFile));
            }
            catch (Exception ex)
            {
                var msg = "Error during download of " + trackingFilePath;
                //todo: update IConsole to support writing errors etc.
                ConsoleHelper.WriteError(msg);
                logger.ErrorException(msg, ex);
                _downloaderFiles.MoveFileAndSettingsFileFromDownloadingFolderToDownloadErrorFolderWriteExceptionFile(dto.GetTrackingNumber(), ex);
            }
        }


        internal EnqueueRequestDTO GetNextImportFileToDownloadMoveToDownloadingOrDefault(HotLogger logger, IConsoleWriter console)
        {
            _downloaderFiles.RefreshFiles();
            string importFileNamePath;
            string importFileName;
            do
            {
                importFileNamePath = _downloaderFiles.DownloadQueueFilePaths.SortByDateAscending().FirstOrDefault();
                if (importFileNamePath == null)
                {
                    logger.Trace("nextFile==null, returning.");
                    return null;
                }
                importFileName = Path.GetFileName(importFileNamePath);
                var status = _downloaderFiles.GetStatusByImportFileName(Path.GetFileName(importFileNamePath));
                // must only have 1 status of queued for downloading or error, otherwise pick the next file
                if(status != QueueStatus.QueuedForDownloading)
                {
                    var msg = string.Format("Skipping {0}, \tstatus is {1}.", importFileName, status);
                    console.WriteLine(msg);
                    logger.Trace(msg);
                    // rename as skipped for now.
                    string destination = Path.Combine(_downloaderFiles.DownloadErrorFolderPath,importFileName + "." + status + EnqueueRequestDTO.SkippedExtension);
                    logger.Trace("Renaming and moving to " + destination);
                    if (File.Exists(destination))
                    {
                        destination = destination.IncrementNumberAtEndOfString();
                    }
                    File.Move(importFileNamePath, destination);
                    importFileNamePath = null;
                    _downloaderFiles.RefreshFiles();
                }
            } while (importFileNamePath==null);

            string json = File.ReadAllText(importFileNamePath);
            EnqueueRequestDTO importFile = JSONHelper.Deserialize<EnqueueRequestDTO>(json);
            _downloaderFiles.MoveImportFileFromDownloadQueueuToDownloading(importFileName);
            return importFile;
        }


    }
}
