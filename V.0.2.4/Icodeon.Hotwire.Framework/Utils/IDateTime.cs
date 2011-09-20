using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Utils
{
    public interface IDateTime
    {
        int SecondsSince1970 { get; }
        DateTime Now { get; }
    }
}
