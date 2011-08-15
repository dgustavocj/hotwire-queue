using System;
using System.Configuration;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class SSLEmailErrorHandlerConfiguration : ConfigurationBase, ISSLEMailErrorHandlerConfig
    {
        public ISSLEMailErrorHandlerConfig ReadConfig()
        {
            return base.ReadConfig<ISSLEMailErrorHandlerConfig>();
        }

        public override string GetConfigurationSectionName()
        {
            return "sslEmailErrorHandler";
        }



        [ConfigurationProperty("fromAddress", IsRequired = true)]
        public string FromAddress
        {
            get { return (string)this["fromAddress"]; }
            set { this["fromAddress"] = value; }
        }


        [ConfigurationProperty("timeoutSeconds", IsRequired = true)]
        public int TimeoutSeconds
        {
            get { return (int)this["timeoutSeconds"]; }
            set { this["timeoutSeconds"] = value; }
        }

        public string[] ToAddresses
        {
            get { return _toAddresses.Split(new[]{','},StringSplitOptions.RemoveEmptyEntries); }
            set { _toAddresses =string.Join(",",value); }
        }

        [ConfigurationProperty("toAddresses", IsRequired = true)]
        private string _toAddresses
        {
            get { return ((string)this["toAddresses"]); }
            set { this["toAddresses"] = value; }
        }

        
        [ConfigurationProperty("subjectLinePrefix", IsRequired = true)]
        public string SubjectLinePrefix
        {
            get { return (string)this["subjectLinePrefix"]; }
            set { this["subjectLinePrefix"] = value; }
        }

 
    }
}
