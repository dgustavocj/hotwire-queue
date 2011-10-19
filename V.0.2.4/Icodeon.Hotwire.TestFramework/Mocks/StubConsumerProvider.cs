using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Contracts;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    
    public class StubConsumerProvider : IConsumerProvider
    {
        public string GetConsumerSecret(string consumerKey)
        {
            switch (consumerKey)
            {
                // development testing
                case "key": return "secret";
                // test java client
                case "key1": return "secret1";
                default: throw new ArgumentOutOfRangeException(consumerKey);
            }
        }
    }

}
