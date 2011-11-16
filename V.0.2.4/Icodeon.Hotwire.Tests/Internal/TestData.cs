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
using Icodeon.Hotwire.TestFramework;

namespace Icodeon.Hotwire.Tests.Internal
{
    public class TestData : TestDataBase
    {
        public const int TestAspNetPort = 44144;

        public TestData(HotwireFilesProvider filesProvider) : base(filesProvider) {}
        public const string TrackingNumber = "B21FCB05-1F88-4956-8FB6-3E5AA579B3F9-alphabet_textfile.txt";

        public static string[] OneTestFile = new[]
                                                 {
                                                     "539EF088-5FE3-4D89-BFE3-DC5F0B8038E7_numbers.txt.import"
                                                 };


        public static string[] ThreeTestImportFilesWithDownloadedContent = new[] {    
                                    "539EF088-5FE3-4D89-BFE3-DC5F0B8038E7_numbers.txt",
                                    "539EF088-5FE3-4D89-BFE3-DC5F0B8038E7_numbers.txt.import",
                                    "999EF088-5FE3-4D89-BFE3-DC5F0B8038E7_hello.txt.import",
                                    "999EF088-5FE3-4D89-BFE3-DC5F0B8038E7_hello.txt",
                                    "B21FCB05-1F88-4956-8FB6-3E5AA579B3F9_alphabet-textfile.txt.import",
                                    "B21FCB05-1F88-4956-8FB6-3E5AA579B3F9_alphabet-textfile.txt" };


        public override string ImportFile
        {
            get { return "B21FCB05-1F88-4956-8FB6-3E5AA579B3F9-alphabet_textfile.import"; }
        }


        public void CreateTestEnqueueRequestImportFile(Guid transactionId, string fileName)
        {
            string externalResourceLinkContent = string.Format("http://localhost:{0}/TestCDN/{1}", TestAspNetPort, fileName);
            string templateFile = Path.Combine(_filesProvider.TestDataFolderPath, "template.import");
            string json = File.ReadAllText(templateFile);
            var dal = new QueueDal(_filesProvider);
            var template = JSONHelper.Deserialize<EnqueueRequestDTO>(json);
            template.ExtResourceLinkContent = externalResourceLinkContent;
            template.TransactionId = transactionId.ToString();
            template.ResourceFile = fileName;
            template.ResourceId = "97DB4DF1-F7AB-4913-A811-DD60F3FE2F1C";
            template.ResourceTitle = "Import test file";
            dal.Save(template,QueueStatus.QueuedForDownloading);
        }

        public void CreateTestProcessImportFileAndMockTestFile(Guid transactionId, string fileName)
        {
            //NB! write the mock downloaded file BEFORE writing the import file, in case there's a process that's watching for the import file, looks for the resourceFile
            // before the test resource file is created.
            string trackingNumber = EnqueueRequestDTO.FormatTrackingNumber(transactionId.ToString(), fileName);
            string testFilePath = Path.Combine(_filesProvider.ProcessQueueFolderPath, trackingNumber);
            File.WriteAllText(testFilePath, "mock test file for " + fileName + " with transactionId " + transactionId.ToString());

            string externalResourceLinkContent = string.Format("http://localhost:{0}/TestCDN/{1}", TestAspNetPort, fileName);
            string templateFile = Path.Combine(_filesProvider.TestDataFolderPath, "template.import");
            string json = File.ReadAllText(templateFile);
            var dal = new QueueDal(_filesProvider);
            var template = JSONHelper.Deserialize<EnqueueRequestDTO>(json);
            template.ExtResourceLinkContent = externalResourceLinkContent;
            template.TransactionId = transactionId.ToString();
            template.ResourceFile = fileName;
            template.ResourceId = "97DB4DF1-F7AB-4913-A811-DD60F3FE2F1C";
            template.ResourceTitle = "Import test file";
            dal.Save(template, QueueStatus.QueuedForProcessing);
        }

    }
}
