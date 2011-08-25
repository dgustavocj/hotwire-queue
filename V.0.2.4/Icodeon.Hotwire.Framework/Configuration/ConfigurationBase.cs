using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public abstract class ConfigurationBase : ConfigurationSection, IConfigurationSection
    {
        public const string SectionGroup = "hotwire";

        public string ConfigurationSectionName { get; set; }


        public abstract string GetConfigurationSectionName();

        protected ConfigurationBase()
        {
            ConfigurationSectionName = GetConfigurationSectionName();
        }

        public virtual T ReadConfig<T>() where T : IConfigurationSection
        {
            string sectionName = String.Format(@"{0}/{1}", SectionGroup, ConfigurationSectionName);
            var config = ConfigurationManager.GetSection(sectionName);
            return (T)config;
        }
    }
}
