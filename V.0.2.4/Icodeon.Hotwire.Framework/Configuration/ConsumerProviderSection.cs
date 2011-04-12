using System;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Providers;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class ConsumerProviderSection : AssemblyProviderBase
    {

        protected override string ProviderName
        {
            get { return "consumerProvider"; }
        }
    } 
} 