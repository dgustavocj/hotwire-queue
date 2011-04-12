using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.ConsoleService
{
    public interface IConsoleServiceConfig
    {
        int Port { get; }
        string UriTemplate { get; }
        Uri Uri();
    }
}
