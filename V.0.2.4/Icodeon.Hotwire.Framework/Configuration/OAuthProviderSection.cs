using System.Configuration;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class OAuthProviderSection : ConfigurationSection, IOAuthProviderConfig
    {

        [ConfigurationProperty("partnerConsumerKey", DefaultValue = "", IsRequired = true)]
        public string PartnerConsumerKey
        {
            get { return (string)this["partnerConsumerKey"]; }
            set { this["partnerConsumerKey"] = value; }
        }

        public static IOAuthProviderConfig ReadConfig()
        {
            return (IOAuthProviderConfig)ConfigurationManager.GetSection(ConfigurationBase.SectionGroup + @"/oAuthProvider");
        }

    } // class
} // namespace 