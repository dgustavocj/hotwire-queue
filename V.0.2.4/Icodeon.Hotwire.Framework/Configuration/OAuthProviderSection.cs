using System.Configuration;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class OAuthProviderSection : ConfigurationSection, IOAuthProviderConfig
    {
        [ConfigurationProperty("assemblyName", DefaultValue = "", IsRequired = true)]
        public string AssemblyName
        {
            get { return (string)this["assemblyName"]; }
            set { this["assemblyName"] = value; }
        }

        [ConfigurationProperty("partnerConsumerKey", DefaultValue = "", IsRequired = true)]
        public string PartnerConsumerKey
        {
            get { return (string)this["partnerConsumerKey"]; }
            set { this["partnerConsumerKey"] = value; }
        }

        [ConfigurationProperty("typeName", DefaultValue = "", IsRequired = true)]
        public string TypeName
        {
            get { return (string)this["typeName"]; }
            set { this["typeName"] = value; }
        }


        public static IOAuthProviderConfig ReadConfig()
        {
            return (IOAuthProviderConfig)ConfigurationManager.GetSection(Constants.Configuration.SectionGroup + @"/oAuthProvider");
        }

    } // class
} // namespace 