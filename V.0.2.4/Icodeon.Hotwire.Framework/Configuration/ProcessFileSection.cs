using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class ProcessFileSection : ConfigurationSection
    {
        [ConfigurationProperty("endpoint", IsKey = false, IsRequired = true)]
        public string Endpoint
        {
            get { return (string)this["endpoint"]; }
            set { this["endpoint"] = value; }
        }


        public string GetEndpoint(string trackingNumber)
        {
            return Endpoint.Replace("{TRACKING-NUMBER}", trackingNumber);
        }

        public static ProcessFileSection ReadConfig()
        {
            string sectionName = string.Format(@"{0}/{1}", Constants.Configuration.SectionGroup, "processFile");
            var config = ConfigurationManager.GetSection(sectionName);
            return (ProcessFileSection)config;
        }
    }
}
