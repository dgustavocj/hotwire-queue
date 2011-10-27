using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Contracts.Enums;
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
        public event EventHandler<ExceptionEventArgs> DownloadException;

        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public FileDownloaderScript(IDownloaderFilesProvider downloaderFilesProvider, IClientDownloader clientDownloader)
        {
            _downloaderFiles = downloaderFilesProvider;
            _clientDownloader = clientDownloader;
        }

        public void Run(IConsoleWriter console, string folderPath)
        {
            Run(console, null);
        }

        public void Run(IConsoleWriter console)
        {
            EnqueueRequestDTO dto = null;
            try
            {
                _isRunning = true;
                while ((dto = GetNextImportFileToDownloadMoveToDownloadingOrDefault(console)) != null)
                {
                    DownloadFile(console, dto);
                }
                console.WriteLine("Nothing left to download, exiting.");
            }
            catch (Exception ex)
            {
                RaiseDownloadError(ex,dto);
            }
            finally
            {
                _isRunning = false;
            }
        }

        private void RaiseDownloadError(Exception ex, EnqueueRequestDTO dto)
        {
            var tempHandler = DownloadException;
            if (tempHandler != null)
            {
                tempHandler(this, new ExceptionEventArgs(ex, ePipeLineSection.FileDownload, dto,null));
            }

        }


        private void DownloadFile(IConsoleWriter console, EnqueueRequestDTO dto)
        {
            string trackingFilePath = Path.Combine(_downloaderFiles.DownloadingFolderPath, dto.GetTrackingNumber());
            File.WriteAllText(trackingFilePath, "");

            // raise downloading event
            var downloading = Downloading;
            if (downloading != null) downloading(this, new EnqueueRequestEventArgs(dto));
            console.Write("Downloading {0}", dto.ResourceFile);

            // Then the ext_resource_link file is downloaded and saved to the process Queue folder
            // And the .import file is moved from downloading to the processQueue folder
            // =============================================================================
            try
            {
                string destination = Path.Combine(_downloaderFiles.ProcessQueueFolderPath, dto.GetTrackingNumber());
                Uri uri = new Uri(dto.ExtResourceLinkContent);
                var downloadResult = _clientDownloader.DownloadFileWithTiming(uri, destination);
                console.WriteLine("\t{0:.00} KB in {1:.0} seconds, {2:.0,15} Kb/s", downloadResult.Kilobytes, downloadResult.Seconds, downloadResult.KbPerSec);
                // move the import file from downloading to process queue folder 
                string importSource = Path.Combine(_downloaderFiles.DownloadingFolderPath, dto.GetImportFileName());
                string importDest = Path.Combine(_downloaderFiles.ProcessQueueFolderPath, dto.GetImportFileName());
                // delete the zero byte file!
                File.Move(importSource, importDest);
                File.Delete(trackingFilePath);
                var downloaded = Downloaded;
                if (downloaded != null) downloaded(this, new FileInfoEventArgs(downloadResult.DownloadedFile));
            }
            catch (Exception ex)
            {
                var msg = "Error during download of " + trackingFilePath;
                //todo: update IConsole to support writing errors etc.
                ConsoleHelper.WriteError(msg);
                _logger.ErrorException(msg, ex);
                _downloaderFiles.MoveFileAndSettingsFileFromDownloadingFolderToDownloadErrorFolderWriteExceptionFile(dto.GetTrackingNumber(), ex);
                RaiseDownloadError(ex,dto);
            }
        }


        internal EnqueueRequestDTO GetNextImportFileToDownloadMoveToDownloadingOrDefault(IConsoleWriter console)
        {
            int maxcount=0;
            string importFileNamePath;
            string importFileName;            
            do
            {
                do
                {
                    _downloaderFiles.RefreshDownloadFiles();
                    importFileNamePath = _downloaderFiles.DownloadQueueFilePaths.SortByDateAscending().FirstOrDefault();

                    if (importFileNamePath == null)
                    {
                        _logger.Trace("nextFile==null, returning.");
                        return null;
                    }
                    importFileName = Path.GetFileName(importFileNamePath);
                    if (string.IsNullOrWhiteSpace(Thread.CurrentThread.Name)) Thread.CurrentThread.Name = Path.GetFileName(importFileName);
                    
                } while (importFileNamePath == null);
                EnqueueRequestDTO importFile = null;
                try
                {
                    var importFileDownloadingPath = _downloaderFiles.MoveImportFileFromDownloadQueueuToDownloading(importFileName);
                    string json = File.ReadAllText(importFileDownloadingPath);
                    importFile = JSONHelper.Deserialize<EnqueueRequestDTO>(json);
                    return importFile;
                }
                catch (FileNotFoundException fnf) { continue; }
                catch (IOException ioex)
                {
                    if (ioex.FileAlreadyExists())
                    {
                        SkipFile(importFileNamePath,QueueStatus.Downloading, importFileName, console);
                        RaiseDownloadError(ioex,importFile);
                    }
                    continue;
                }

            } while (maxcount++<30000);
            throw new ApplicationException("safetynet loop value maxcount exceeded! Possible causes would be if were getting 'false negative' IOExceptions, which are supposed to only happen if another thread has moved a file, and the file still exists and the code tries to move the same file again.");
        }

        private void SkipFile(string importFileNamePath, QueueStatus status, string importFileName, IConsoleWriter console)
        {
            var msg = string.Format("Skipping {0}, \tstatus is {1}.", importFileName, status);
            console.WriteLine(msg);
            _logger.Trace(msg);
            // rename as skipped for now.
            string destination = Path.Combine(_downloaderFiles.DownloadErrorFolderPath, importFileName + "." + status + EnqueueRequestDTO.SkippedExtension);
            _logger.Trace("Renaming and moving to " + destination);
            if (File.Exists(destination))
            {
                destination = destination.IncrementNumberAtEndOfString();
            }
            try
            {
                File.Move(importFileNamePath, destination);
            }
            catch (IOException ioex) { } // it's possible another thread could have moved this file, which is a valid scenario.
            _downloaderFiles.RefreshFiles();
        }
    }
}
