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

        [ConfigurationProperty("assemblyName", DefaultValue = "", IsRequired = true)]
        public string AssemblyName
        {
            get { return (string)this["assemblyName"]; }
            set { this["assemblyName"] = value; }
        }

        [ConfigurationProperty("typeName", DefaultValue = "", IsRequired = true)]
        public string TypeName
        {
            get { return (string)this["typeName"]; }
            set { this["typeName"] = value; }
        }

        public static IFileProcessorSection ReadConfig()
        {
            return (FileProcessorSection) ConfigurationManager.GetSection(Constants.Configuration.SectionGroup + @"/fileProcessor");
        }

    } // class
} // namespace 