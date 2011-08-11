using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class ProcessFileConfiguration : ModuleConfigurationBase
    {

        //public static new ProcessFileConfiguration ReadConfig()
        //{
        //    string sectionName = string.Format(@"{0}/{1}", Constants.Configuration.SectionGroup, "processFile");
        //    var config = ConfigurationManager.GetSection(sectionName);
        //    return (ProcessFileConfiguration)config;
        //}

        protected override string GetConfigurationSectionName()
        {
            return Constants.Configuration.ProcessFileSectionName;
        }
    }
}
