using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class QueueConfiguration : ModuleConfigurationBase
    {




        [ConfigurationProperty("requiredParameters", IsRequired = false)]
        public string RequiredParameters
        {
            get { return (string)this["requiredParameters"]; }
            set { this["requiredParameters"] = value; }
        }

        public override string GetConfigurationSectionName()
        {
            return Constants.Configuration.QueuesSectionName;
        }
    }
}
