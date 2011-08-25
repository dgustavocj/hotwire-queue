using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class QueueConfiguration : ModuleConfigurationBase
    {
        public const string QueuesSectionName = "queues";

        [ConfigurationProperty("requiredParameters", IsRequired = false)]
        public string RequiredParameters
        {
            get { return (string)this["requiredParameters"]; }
            set { this["requiredParameters"] = value; }
        }

        public override string GetConfigurationSectionName()
        {
            return QueuesSectionName;
        }
    }
}
