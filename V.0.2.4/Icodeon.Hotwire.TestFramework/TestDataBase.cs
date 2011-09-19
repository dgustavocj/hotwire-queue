using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Repositories;
using Icodeon.Hotwire.Framework.Serialization;
using Icodeon.Hotwire.Framework.Utils;


namespace Icodeon.Hotwire.TestFramework
{
    abstract public class TestDataBase
    {
        private readonly HotwireFilesProvider _filesProvider;

        public abstract string ImportFile { get;  }

        protected TestDataBase(HotwireFilesProvider filesProvider)
        {
            _filesProvider = filesProvider;
        }

        public void WriteTestEnqueueRequestToFolderAndPause(QueueStatus folder, string transactionId, string resourceId, int millisecondsToPause)
        {
            string templateFile = Path.Combine(_filesProvider.TestDataFolderPath, "template.import");
            string json = File.ReadAllText(templateFile);
            var dal = new QueueDal(_filesProvider);
            var template = JSONHelper.Deserialize<EnqueueRequestDTO>(json);
            template.TransactionId = transactionId;
            template.ResourceFile = resourceId;
            template.ResourceId = resourceId;
            template.ResourceTitle = "Test file " + resourceId;
            dal.Save(template, folder);
            Thread.Sleep(millisecondsToPause);
        }


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

        public void CopyImportFileToDownloadErrorFolder()
        {
            var importFilesrc = Path.Combine(_filesProvider.TestDataFolderPath, ImportFile);
            var importFileDestPath = Path.Combine(_filesProvider.DownloadErrorFolderPath, EnqueueRequestDTO.AddErrorExtension(ImportFile));
            File.Copy(importFilesrc, importFileDestPath);
        }

    }
}