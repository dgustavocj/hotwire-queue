using System.Configuration;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class FileProcessorSection : ConfigurationSection, IFileProcessorSection
    {
        [ConfigurationProperty("maxFileProcessorWorkers", DefaultValue = 3, IsRequired = true)]
        public int MaxFileProcessorWorkers
        {
            get { return (int)this["maxFileProcessorWorkers"]; }
            set { this["maxFileProcessorWorkers"] = value; }
        }

        public static IFileProcessorSection ReadConfig()
        {
            return (FileProcessorSection) ConfigurationManager.GetSection(Constants.Configuration.SectionGroup + @"/fileProcessor");
        }

    } // class
} // namespace 