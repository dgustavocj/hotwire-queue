namespace Icodeon.Hotwire.Framework.Contracts
{
    public class Uris
    {
        // bit messy not to have the Uri's quick and easy to read in the service contracts.
        // will have to keep moving eyes between the Uris class and the IHotwireService interface when reading the code.
        // makes the code far less "scannable". 
        public class Ver0_2
        {
            public const string VersionFramework_GET = "version-framework";
            public const string VersionServiceHost_GET = "version-servicehost";
            public const string QueuedCartridges_xml_POST = "{module}/QueuedCartridges.xml";
            
            // removed the .xml so that there's no suggestion that this will support.json or other formats.
            public const string HotwireModules_FileProcessor = "HotwireModules/FileProcessor";


            public class HotwireTests
            {
                //public const string Tests_TestDummyFileProcessor = "/Tests/TestDummyFileProcessor"; 
                public const string HotwireTests_Echo = "/HotwireTests/Echo";
                public const string HotwireTests_FileMove = "/HotwireTests/FileMove";
                public const string HotwireTests_BackgroundFileMove = "/HotwireTests/BackgroundFileMove";
            }

        }



    }
}