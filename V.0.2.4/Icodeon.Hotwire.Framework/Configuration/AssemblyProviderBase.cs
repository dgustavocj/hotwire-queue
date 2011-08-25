using System.Configuration;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public abstract class AssemblyProviderBase : ConfigurationSection, IAssemblyProvider
    {

        [ConfigurationProperty("assemblyName", IsRequired = true)]
        public string AssemblyName
        {
            get { return (string)this["assemblyName"]; }
            set { this["assemblyName"] = value; }
        }

        [ConfigurationProperty("typeName", IsRequired = true)]
        public string TypeName
        {
            get { return (string)this["typeName"]; }
            set { this["typeName"] = value; }
        }

        protected abstract string ProviderName { get; }

        public IAssemblyProvider ReadConfig()
        {
            return (IAssemblyProvider)ConfigurationManager.GetSection(ConfigurationBase.SectionGroup + @"/" + ProviderName);
        }


        

    } // class
} // namespace 