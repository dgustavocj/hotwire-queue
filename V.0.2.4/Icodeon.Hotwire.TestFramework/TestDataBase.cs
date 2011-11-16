using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.DAL;
using Icodeon.Hotwire.Framework.Serialization;
using Icodeon.Hotwire.Framework.Utils;


namespace Icodeon.Hotwire.TestFramework
{
    abstract public class TestDataBase
    {
        protected readonly HotwireFilesProvider _filesProvider;

        public abstract string ImportFile { get;  }

        protected TestDataBase(HotwireFilesProvider filesProvider)
        {
            _filesProvider = filesProvider;
        }


        public const string ImportJsonTemplate = @" {   
            'ExtResourceLinkContent':'@RESOURCE-DOWNLOAD-URL@',
            'ExtraParameters':[{'Name':'fruit1','Value':'apple'}, {'Name':'fruit2','Value':'banana'}],
            'HotwireVersion':'0.1',
            'QueueCategory':'TestCategory',
            'QueueName':null,
            'QueuePriority':2,
            'QueueStatus':0,
            'ResourceFile':'@RESOURCE-FILE@',
            'ResourceId':'@RESOURCE-ID@',
            'ResourceTitle':'@RESOURCE-TITLE@',
            'TransactionId':'@TRANSACTION-ID@',
            'UserId':'hotwire-testing' }";


        public void CopyFilesToProcessQueueFolder(IEnumerable<string> processFileNames, IConsoleWriter console)
        {
            processFileNames.ToList().ForEach(importFile =>
                                                  {
                                                      var importFilesrc = Path.Combine(_filesProvider.TestDataFolderPath, importFile);
                                                      var importFileDestPath = Path.Combine(_filesProvider.ProcessQueueFolderPath, importFile);
                                                      console.WriteLine("copying {0}", importFile);
                                                      File.Copy(importFilesrc, importFileDestPath);
                                                  });
        }

        
        public bool FileExistsInDownloadQueueFolder(string fileName)
        {
            var filePath = Path.Combine(_filesProvider.DownloadQueueFolderPath, fileName);
            return File.Exists(filePath);
        }

        public bool FileExistsInProcessFileFolder(string fileName)
        {
            var filePath = Path.Combine(_filesProvider.ProcessedFolderPath, fileName);
            return File.Exists(filePath);
        }


        public void CopyFilesToDownloadQueueFolder(IEnumerable<string> importFileNames, IConsoleWriter console)
        {
            importFileNames.ToList().ForEach(importFile =>
                                                 {
                                                     var importFilesrc = Path.Combine(_filesProvider.TestDataFolderPath, importFile);
                                                     var importFileDestPath = Path.Combine(_filesProvider.DownloadQueueFolderPath, importFile);
                                                     console.WriteLine("copying {0}", importFile);
                                                     File.Copy(importFilesrc, importFileDestPath);
                                                 });
        }

        // cause the files times to be at least 1 second apart
        public void CopyFilesSlowlyToDownloadQueueFolder(IEnumerable<string> importFileNames, IConsoleWriter console)
        {
            importFileNames.ToList().ForEach(importFile =>
                                                 {
                                                     var importFilesrc = Path.Combine(_filesProvider.TestDataFolderPath, importFile);
                                                     var importFileDestPath = Path.Combine(_filesProvider.DownloadQueueFolderPath, importFile);
                                                     File.Copy(importFilesrc, importFileDestPath);
                                                     console.WriteLine("copying {0}", importFile);
                                                     Thread.Sleep(1200);
                                                 });
        }

        public void CopyImportFileToDownloadQueueFolder()
        {
            var importFilesrc = Path.Combine(_filesProvider.TestDataFolderPath, ImportFile);
            var importFileDestPath = Path.Combine(_filesProvider.DownloadQueueFolderPath, ImportFile);
            File.Copy(importFilesrc, importFileDestPath);
        }


        public void CopyImportFileToDownloadingFolder()
        {
            var importFilesrc = Path.Combine(_filesProvider.TestDataFolderPath, ImportFile);
            var importFileDestPath = Path.Combine(_filesProvider.DownloadingFolderPath, ImportFile);
            File.Copy(importFilesrc, importFileDestPath);
        }

        public void CopyImportFileToProcessedFolder()
        {
            var importFilesrc = Path.Combine(_filesProvider.TestDataFolderPath, ImportFile);
            var importFileDestPath = Path.Combine(_filesProvider.ProcessedFolderPath, ImportFile);
            File.Copy(importFilesrc, importFileDestPath);
        }


        public void CopyImportFileToProcessQueueFolder()
        {
            var importFilesrc = Path.Combine(_filesProvider.TestDataFolderPath, ImportFile);
            var importFileDestPath = Path.Combine(_filesProvider.ProcessQueueFolderPath, ImportFile);
            File.Copy(importFilesrc, importFileDestPath);
        }

        public void CreateFakeDownloadErrorFileFromImportNumber()
        {
            string trackingNumber = EnqueueRequestDTO.GetTrackingNumberFromImportFile(ImportFile);
            var downloadErrorFileDest = Path.Combine(_filesProvider.DownloadErrorFolderPath, EnqueueRequestDTO.AddErrorExtension(trackingNumber));
            File.WriteAllText(downloadErrorFileDest,"this is a fake error file (404) blah");
        }

    }
}