using System.Configuration;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class FoldersSection : ConfigurationSection, IHotwireFileProcessorRelativeFolders
    {
        public static FoldersSection ReadConfig()
        {
            return (FoldersSection)ConfigurationManager.GetSection(ConfigurationBase.SectionGroup + @"/folders");
        }

        [ConfigurationProperty("processQueueFolder", IsRequired = true)]
        public string ProcessQueueFolder
        {
            get { return (string)this["processQueueFolder"]; }
            set { this["processQueueFolder"] = value; }
        }

        [ConfigurationProperty("downloadErrorFolder", IsRequired = true)]
        public string DownloadErrorFolder
        {
            get { return (string)this["downloadErrorFolder"]; }
            set { this["downloadErrorFolder"] = value; }
        }

        [ConfigurationProperty("downloadQueueFolder", IsRequired = true)]
        public string DownloadQueueFolder
        {
            get { return (string)this["downloadQueueFolder"]; }
            set { this["downloadQueueFolder"] = value; }
        }


        [ConfigurationProperty("processedFolder", IsRequired = true)]
        public string ProcessedFolder
        {
            get { return (string)this["processedFolder"]; }
            set { this["processedFolder"] = value; }
        }

        [ConfigurationProperty("processingFolder", IsRequired = true)]
        public string ProcessingFolder
        {
            get { return (string)this["processingFolder"]; }
            set { this["processingFolder"] = value; }
        }

        [ConfigurationProperty("processErrorFolder", IsRequired = true)]
        public string ProcessErrorFolder
        {
            get { return (string)this["processErrorFolder"]; }
            set { this["processErrorFolder"] = value; }
        }

        [ConfigurationProperty("downloadingFolder", IsRequired = true)]
        public string DownloadingFolder
        {
            get { return (string)this["downloadingFolder"]; }
            set { this["downloadingFolder"] = value; }
        }

        [ConfigurationProperty("solutionFolderMarkerFile", IsRequired = true)]
        public string SolutionFolderMarkerFile
        {
            get { return (string)this["solutionFolderMarkerFile"]; }
            set { this["solutionFolderMarkerFile"] = value; }
        }

        // set this to true : if set to "" then the FilesProvider will revert to the root directory! Dangerouse if you have a clearout function!
        [ConfigurationProperty("testDataFolder", IsRequired = true)]
        public string TestDataFolder
        {
            get { return (string)this["testDataFolder"]; }
            set { this["testDataFolder"] = value; }
        }

    } // class
} // namespace 