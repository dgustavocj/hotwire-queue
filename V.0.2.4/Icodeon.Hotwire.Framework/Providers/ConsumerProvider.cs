using System;
using Icodeon.Hotwire.Contracts;

namespace Icodeon.Hotwire.Framework.Providers
{
    public class ConsumerProvider : IConsumerProvider
    {
        public const string Key = "key";
        public const string Secret = "secret";
        public string SecretOther = "other";
        public string GetConsumerSecret(string consumerKey)
        {
            
            return consumerKey.Equals(Key, StringComparison.InvariantCultureIgnoreCase)
                       ? Secret
                       : SecretOther;
        }
    }
}
