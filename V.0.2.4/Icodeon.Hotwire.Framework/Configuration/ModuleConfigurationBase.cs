using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public abstract class ModuleConfigurationBase : ConfigurationSection, IModuleConfiguration
    {
        public string ConfigurationSectionName { get; set; }

        protected abstract string GetConfigurationSectionName();

        public ModuleConfigurationBase()
        {
            ConfigurationSectionName = GetConfigurationSectionName();
        }

        public IModuleConfiguration ReadConfig()
        {
            string sectionName = string.Format(@"{0}/{1}", Constants.Configuration.SectionGroup, ConfigurationSectionName);
            var config = ConfigurationManager.GetSection(sectionName);
            return (IModuleConfiguration)config;
        }

        [ConfigurationProperty("rootServiceName", IsRequired = true)]
        public string RootServiceName
        {
            get { return (string)this["rootServiceName"]; }
            set { this["rootServiceName"] = value; }
        }

        [ConfigurationProperty("methodValidation", IsRequired = false,DefaultValue = MethodValidation.beforeUriValidation)]
        public MethodValidation MethodValidation
        {
            get { return (MethodValidation)this["methodValidation"]; }
            set { this["methodValidation"] = value; }
        }

        [ConfigurationProperty("active", IsRequired = true)]
        public bool Active
        {
            get { return (bool)this["active"]; }
            set { this["active"] = value; }
        }


        public List<IModuleEndpoint> Endpoints
        {
            get
            {
                var accounts = EndpointCollection.Cast<IModuleEndpoint>().ToList();
                return accounts;
            }
        }

        // this attributes defines the xml collection element name
        [ConfigurationProperty("endpoints")]
        public EndpointCollection EndpointCollection
        {
            get { return (EndpointCollection)this["endpoints"]; }
        }








    } // class
} // namespace 