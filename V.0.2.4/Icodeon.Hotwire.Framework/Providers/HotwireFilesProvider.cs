using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Serialization;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Providers
{

    //TODO: Create repository pattern and update filesProvider to simply be a repository implementing IHotwireRepository

    // moving to provider namespace, preparing for being able to swap out the file provider
    // will need to extract the full interface and make it pluggable
    public class HotwireFilesProvider : IHotwireFileProcessorFoldersPaths, IProcessFiles 
    {

        public static class MarkerFiles
        {
            public const string SolutionFolder = "HotwireSolutionFolderMarkerFile.txt";
            public const string ProcessingFolder = "marker-processing.txt";
            public const string DownloadErrorFolder = "marker-downloadError.txt";
            public const string DownloadingFolder = "marker-downloading.txt";
            public const string DownloadQueueFolder = "marker-downloadQueue.txt";
            public const string ProcessedFolder = "marker-processed.txt";
            public const string ProcessErrorFolder = "marker-processError.txt";
            public const string ProcessQueueFolder = "marker-processQueue.txt";
            public static IEnumerable<string> Files = new[] { SolutionFolder, ProcessingFolder, DownloadErrorFolder, DownloadingFolder, DownloadQueueFolder, ProcessedFolder, ProcessErrorFolder, ProcessQueueFolder };
        }




        private readonly LoggerBase _logger;

        //ADH: looks like _folderPaths is never used, need to check.
        private IHotwireFileProcessorFoldersPaths _foldersPaths;

        // TODO: DRY, am repeating myself here... fix!
        public IEnumerable<string> TestDataFilePaths { get; private set; }
        public IEnumerable<string> DownloadQueueFilePaths { get; private set; }
        public IEnumerable<string> DownloadingFilePaths { get; private set; }
        public IEnumerable<string> ProcessQueueFilePaths { get; private set; }
        public IEnumerable<string> ProcessedFilePaths { get; private set; }
        public IEnumerable<string> ProcessingFilePaths { get; private set; }
        public IEnumerable<string> ProcessErrorFilePaths { get; private set; }
        public IEnumerable<string> DownloadErrorFilePaths { get; private set; }

        
        public string SolutionFolderMarkerFile { get; set; }

        public string TestDataFolderPath { get; set; }
        public string DownloadErrorFolderPath { get; set; }
        public string DownloadQueueFolderPath { get; set; }
        public string DownloadingFolderPath { get; set; }
        public string ProcessQueueFolderPath { get; set; }
        public string ProcessedFolderPath { get; set; }
        public string ProcessingFolderPath { get; set; }
        public string ProcessErrorFolderPath { get; set; }

        public HotwireFilesProvider(IHotwireFileProcessorRelativeFolders relativeFolders, LoggerBase logger)
        {
            _logger = logger;
            SolutionFolderMarkerFile = relativeFolders.SolutionFolderMarkerFile;
            string rootPath = DirectoryHelper.GetSolutionRootPath(relativeFolders.SolutionFolderMarkerFile);

            TestDataFolderPath = Path.Combine(rootPath, relativeFolders.TestDataFolder);
            DownloadErrorFolderPath = Path.Combine(rootPath, relativeFolders.DownloadErrorFolder);
            DownloadQueueFolderPath = Path.Combine(rootPath, relativeFolders.DownloadQueueFolder);
            ProcessQueueFolderPath = Path.Combine(rootPath, relativeFolders.ProcessQueueFolder);
            ProcessedFolderPath = Path.Combine(rootPath, relativeFolders.ProcessedFolder);
            ProcessingFolderPath = Path.Combine(rootPath, relativeFolders.ProcessingFolder);
            ProcessErrorFolderPath = Path.Combine(rootPath, relativeFolders.ProcessErrorFolder);
            DownloadingFolderPath = Path.Combine(rootPath, relativeFolders.DownloadingFolder);
            RefreshFiles();
        }


        public ImportCartridgeDTO GetBulkImportSettingsForNextFileToProcessAndMoveFileToProcessingOrNull()
        {
            _logger.Trace("\t\tGetBulkImportSettingsForNextFileToProcessAndMoveFileToProcessingOrNull");
            _logger.Trace("\t\tRefreshFiles()");
            RefreshFiles();
            if (ProcessQueueFilePaths.Count() == 0)
            {
                _logger.Trace("\t\tNo files in processing queue to process, returning.");
                return null;
            }
            var nextFile = Path.GetFileName(ProcessQueueFilePaths.First());
            _logger.Trace("\t\tnextFile:{0}", nextFile);
            if (nextFile == null)
            {
                _logger.Trace("\t\tnextFile is null, returning.");
                return null;
            }
            var nextFileSettings = EnqueueRequestDTO.AddImportExtension(nextFile);
            string nextFileSettingsPath = Path.Combine(ProcessQueueFolderPath, nextFileSettings);
            _logger.Trace("\t\tnextFileSettingsPath={0}", nextFileSettingsPath);
            _logger.Trace("\t\tParsing the .import file");
            ImportCartridgeDTO cartridgeDto = ImportCartridgeDTO.ReadImportSettingsFile(nextFileSettingsPath);
            _logger.Trace("\t\tMoveFileAndSettingsFileToFolder('{0}','{1}','{2}',logger)", nextFile, ProcessQueueFolderPath, ProcessingFolderPath);
            MoveFileAndSettingsFileToFolder(nextFile,ProcessQueueFolderPath, ProcessingFolderPath);
            return cartridgeDto;
        }

        public void EmptyProcessQueueFolder()
        {
            DirectoryHelper.DeleteAllFilesExceptMarker(ProcessQueueFolderPath, MarkerFiles.ProcessQueueFolder);
        }

        public void EmptyProcessedFolder()
        {
            DirectoryHelper.DeleteAllFilesExceptMarker(ProcessedFolderPath, MarkerFiles.ProcessedFolder);
        }


        public void EmptyProcessingFolder()
        {
            DirectoryHelper.DeleteAllFilesExceptMarker(ProcessingFolderPath, MarkerFiles.ProcessingFolder);
        }

        public void EmptyDownloadQueueuFolder()
        {
            DirectoryHelper.DeleteAllFilesExceptMarker(DownloadQueueFolderPath, MarkerFiles.DownloadQueueFolder);
        }


        public void EmptyProcessErrorFolder()
        {
            DirectoryHelper.DeleteAllFilesExceptMarker(ProcessErrorFolderPath, MarkerFiles.ProcessErrorFolder);
        }


        public void EmptyDownloadErrorFolder()
        {
            DirectoryHelper.DeleteAllFilesExceptMarker(DownloadErrorFolderPath , MarkerFiles.DownloadErrorFolder);
        }


        public void EmptyDownloadingFolder()
        {
            DirectoryHelper.DeleteAllFilesExceptMarker(DownloadingFolderPath, MarkerFiles.DownloadingFolder);
        }

        public string GetFolderByStatus(QueueStatus status)
        {
            switch (status)
            {
                case QueueStatus.QueuedForDownloading:  return DownloadQueueFolderPath;
                case QueueStatus.QueuedForProcessing:   return ProcessQueueFolderPath;
                case QueueStatus.Downloading:           return DownloadingFolderPath;
                case QueueStatus.Processing:            return ProcessingFolderPath;
                case QueueStatus.Processed:             return ProcessedFolderPath;
                case QueueStatus.ProcessError:          return ProcessErrorFolderPath;
                case QueueStatus.DownloadError:         return DownloadErrorFolderPath;
                default:
                    throw new ArgumentOutOfRangeException("No folder for status '{0}' is configured." + status);
            }

        }

        public void MoveImportFileFromDownloadQueueuToDownloading(string importFileName)
        {
            MoveImportFileToFolder(importFileName, DownloadQueueFolderPath, DownloadingFolderPath);
        }


        public void MoveFileFromDownloadToProcessQueue(string filename)
        {
            MoveFileAndSettingsFileToFolder(filename, DownloadingFolderPath, ProcessQueueFolderPath);
        }

        private void MoveImportFileToFolder(string importFile, string srcFolderPath, string destFolderPath)
        {
            // move settings file
            var importSrcFile = Path.Combine(srcFolderPath, importFile);
            var importDestFile = Path.Combine(destFolderPath, importFile);
            _logger.Trace("\t\t\tFile.Move('{0}',{1}')", importSrcFile, importDestFile);
            File.Move(importSrcFile, importDestFile);
        }


        public void MoveFileAndSettingsFileToFolder(string trackingNumber, string srcFolderPath, string destFolderPath)
        {
            // move file
            var filesrc = Path.Combine(srcFolderPath, trackingNumber);
            var filedest = Path.Combine(destFolderPath, trackingNumber);
            _logger.Trace("\t\t\tFile.Move('{0}',{1}')",filesrc, filedest);
            File.Move(filesrc, filedest);
            
            // move settings file
            var settingFilename = EnqueueRequestDTO.AddImportExtension(trackingNumber);
            var settingSrcPath = Path.Combine(srcFolderPath, settingFilename);
            var settingDestPath = Path.Combine(destFolderPath, settingFilename);
            _logger.Trace("\t\t\tFile.Move('{0}',{1}')", settingSrcPath, settingDestPath);
            File.Move(settingSrcPath, settingDestPath);
        }

        public void MoveFileAndSettingsFileFromProcessingFolderToProcessedFolderLogSuccess(string resourceFile)
        {
            _logger.Trace("MoveFileAndSettingsFileFromProcessingFolderToProcessedFolderLogSuccess(...)");
            MoveFileAndSettingsFileToFolder(resourceFile, ProcessingFolderPath, ProcessedFolderPath);
            _logger.Info("\t\t\tprocessed {0}.", resourceFile);
        }   

        public void MoveFileAndSettingsFileFromProcessingFolderToErrorFolderWriteExceptionFile(string trackingNumber, Exception ex)
        {
            _logger.Trace("MoveFileAndSettingsFileFromProcessingFolderToErrorFolderWriteExceptionFile(...)");
            MoveFileAndSettingsFileToFolder(trackingNumber, ProcessingFolderPath, ProcessErrorFolderPath);
            _logger.Trace("\t\t\tWriting .errorFile", trackingNumber);
            // article on serializing exception to file
            // http://seattlesoftware.wordpress.com/2008/08/22/serializing-exceptions-to-xml/
            
            // log exception to json error file
            string jsonException = JSONHelper.Serialize(new HotwireExceptionDTO(ex));
            string errorFilePath = Path.Combine(ProcessErrorFolderPath, EnqueueRequestDTO.AddErrorExtension(trackingNumber));
            File.WriteAllText(errorFilePath,jsonException);
        }


        //TODO: DRY below is repeat of above!
        public void MoveFileAndSettingsFileFromDownloadingFolderToDownloadErrorFolderWriteExceptionFile(string trackingNumber, Exception ex)
        {
            _logger.Trace("MoveFileAndSettingsFileFromDownloadingFolderToDownloadErrorFolderWriteExceptionFile('{0}',exception)",trackingNumber);
            MoveFileAndSettingsFileToFolder(trackingNumber, DownloadingFolderPath, DownloadErrorFolderPath);
            _logger.Trace("\t\t\tWriting .errorFile", trackingNumber);
            // log exception to json error file
            string jsonException = JSONHelper.Serialize(new HotwireExceptionDTO(ex));
            string errorFilePath = Path.Combine(DownloadErrorFolderPath, EnqueueRequestDTO.AddErrorExtension(trackingNumber));
            File.WriteAllText(errorFilePath, jsonException);
            // dont log the actual error to the log file, that will be done by the caller's outermost try catch, i.e. most likely the Script Runner.
        }

        //TODO: Rename the XYZFilePaths to ImportFiles!
        public void RefreshFiles()
        {
            TestDataFilePaths = Directory.GetFiles(TestDataFolderPath).ToList();
            DownloadQueueFilePaths = Directory.GetFiles(DownloadQueueFolderPath).Where(f => f.EndsWith(EnqueueRequestDTO.ImportExtension)).ToList();
            ProcessQueueFilePaths = Directory.GetFiles(ProcessQueueFolderPath).Where(f => f.EndsWith(EnqueueRequestDTO.ImportExtension)).ToList();
            ProcessedFilePaths = Directory.GetFiles(ProcessedFolderPath).Where(f => f.EndsWith(EnqueueRequestDTO.ImportExtension)).ToList();
            ProcessingFilePaths = Directory.GetFiles(ProcessingFolderPath).Where(f => f.EndsWith(EnqueueRequestDTO.ImportExtension)).ToList();
            DownloadingFilePaths = Directory.GetFiles(DownloadingFolderPath).Where(f => f.EndsWith(EnqueueRequestDTO.ImportExtension)).ToList();
            ProcessErrorFilePaths = Directory.GetFiles(ProcessErrorFolderPath).Where(f => f.EndsWith(EnqueueRequestDTO.ErrorExtension) || f.EndsWith(EnqueueRequestDTO.SkippedExtension)).ToList();
            DownloadErrorFilePaths = Directory.GetFiles(DownloadErrorFolderPath).Where(f => f.EndsWith(EnqueueRequestDTO.ErrorExtension) || f.EndsWith(EnqueueRequestDTO.SkippedExtension) ).ToList();
        }

        public IProcessFiles GetFiles()
        {
            throw new NotImplementedException();
        }



        public List<QueueStatus> GetStatuses(string importFilename)
        {
            var statuses = new List<QueueStatus>();
            if (DownloadQueueFilePaths != null && DownloadQueueFilePaths.ContainsFile(importFilename))  statuses.Add(QueueStatus.QueuedForDownloading);
            if (DownloadingFilePaths != null && DownloadingFilePaths.ContainsFile(importFilename))      statuses.Add(QueueStatus.Downloading);
            if (ProcessQueueFilePaths != null && ProcessQueueFilePaths.ContainsFile(importFilename))    statuses.Add(QueueStatus.QueuedForProcessing);
            if (ProcessingFilePaths != null && ProcessingFilePaths.ContainsFile(importFilename))        statuses.Add(QueueStatus.Processing);
            if (ProcessedFilePaths != null && ProcessedFilePaths.ContainsFile(importFilename))          statuses.Add(QueueStatus.Processed);
            if (DownloadErrorFilePaths != null && DownloadErrorFilePaths.ContainsFile(importFilename)) statuses.Add(QueueStatus.DownloadError);
            if (ProcessErrorFilePaths != null && ProcessErrorFilePaths.ContainsFile(importFilename))    statuses.Add(QueueStatus.ProcessError);
            if (statuses.Count()==0) statuses.Add(QueueStatus.None);
            return statuses;
        }

        public QueueStatus GetStatusByTrackingNumber(string trackingNumber)
        {
            string importFileName = EnqueueRequestDTO.AddImportExtension(trackingNumber);
            var status = GetStatusByImportFileName(importFileName);
            return status;
        }

        public QueueStatus GetStatusByImportFileName(string importFileName)
        {
            // need to lookup status for download queue folder differently because at this point we only have an enqueue request file and no resource file as it has not been downloaded yet!
            if (ProcessedFilePaths != null && ProcessedFilePaths.ContainsFile(importFileName))         return QueueStatus.Processed;
            if (ProcessingFilePaths != null && ProcessingFilePaths.ContainsFile(importFileName)) return QueueStatus.Processing;
            if (ProcessQueueFilePaths != null && ProcessQueueFilePaths.ContainsFile(importFileName)) return QueueStatus.QueuedForProcessing;
            if (DownloadingFilePaths != null && DownloadingFilePaths.ContainsFile(importFileName)) return QueueStatus.Downloading;
            if (DownloadQueueFilePaths != null && DownloadQueueFilePaths.ContainsFile(importFileName)) return QueueStatus.QueuedForDownloading;
            if (ProcessErrorFilePaths != null && ProcessErrorFilePaths.ContainsErrorFile(importFileName)) return QueueStatus.ProcessError;
            if (DownloadErrorFilePaths != null && DownloadErrorFilePaths.ContainsErrorFile(importFileName)) return QueueStatus.DownloadError;
            return QueueStatus.None;
        }


        public IEnumerable<string> GetAllFilesExcludingMarkerFiles(string folderPath)
        {
            var files = Directory.GetFiles(folderPath, "*.*").Where( f => !MarkerFiles.Files.Any( f.Contains ) );
            return files;
        }

        private static HotwireFilesProvider _hotwireFilesProvider;
        public static HotwireFilesProvider GetFilesProviderInstance(LoggerBase logger)
        {
            logger.Trace("HotwireFilesProvider GetFilesProviderInstance(...)");
            logger.Trace(_hotwireFilesProvider == null ? "creating new instance of HotwireFilesProvider" : "reading  filesProvider config");
            return _hotwireFilesProvider ?? (_hotwireFilesProvider = new HotwireFilesProvider(FoldersSection.ReadConfig(), logger));
        }



    }
}
