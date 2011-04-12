using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public interface IAssemblyProvider
    {
        string AssemblyName { get; }
        string TypeName { get; }
    }
}
