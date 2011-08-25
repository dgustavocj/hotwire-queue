using System;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public static class Constants
    {
        public static class TemporaryKeyAndSecretLookups
        {
            private static string _partnerConsumerKey = null;
            public static string PartnerConsumerKey
            {
                get
                {
                    if (_partnerConsumerKey == null) _partnerConsumerKey = OAuthProviderSection.ReadConfig().PartnerConsumerKey;
                    return _partnerConsumerKey;
                }
            }
            // for testing ... a value that is definately NOT the partner consumer key.
            public const string NotPartnerConsumerKey = "d933ce9a8d273de4309441e5031585e4";
        }

        public static class ProjectFolders
        {
            public const string Icodeon_Hotwire_TestAspNet = "Icodeon.OUIntegration.TestAspNet";
        }


        public static class ProjectPorts
        {
            public const int TestAspNet = 54144;
        }

        public static class OAuth
        {
            public const string oauth_consumer_key = "oauth_consumer_key";

            public static class RequestParameterNames
            {
                public const string USER_ID = "user_id";
            }
        }
        public static class Namespaces
        {
            public const string ICODEON_HOTWIRE_BETA_V0_2 = "http://www.icodeon.com/hotwire/beta/v0.2";    
        }
        
    }
}