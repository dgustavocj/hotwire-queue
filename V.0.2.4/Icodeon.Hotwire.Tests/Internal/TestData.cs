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



    }
}
