using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public abstract class ConfigurationBase : ConfigurationSection, IConfigurationSection
    {
        public string ConfigurationSectionName { get; set; }


        public abstract string GetConfigurationSectionName();

        protected ConfigurationBase()
        {
            ConfigurationSectionName = GetConfigurationSectionName();
        }

        public virtual T ReadConfig<T>() where T : IConfigurationSection
        {
            string sectionName = string.Format(@"{0}/{1}", Constants.Configuration.SectionGroup, ConfigurationSectionName);
            var config = ConfigurationManager.GetSection(sectionName);
            return (T)config;
        }



    }
}
