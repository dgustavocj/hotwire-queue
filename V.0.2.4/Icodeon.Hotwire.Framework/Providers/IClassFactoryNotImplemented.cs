using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Providers
{
    // provided here as part of the framework so that you can run live testing on your IOC framework.
    // see Icodeon.Hotwire.Tests.ProviderFactoryTests.cs
    public interface IClassFactoryNotImplemented
    {
        string Greet();
    }

    public class DefaultForClassFactoryNotImplemented : IClassFactoryNotImplemented
    {
        public string Greet()
        {
            return "I am a DefaultForClassFactoryNotImplemented";
        }
    }
}
