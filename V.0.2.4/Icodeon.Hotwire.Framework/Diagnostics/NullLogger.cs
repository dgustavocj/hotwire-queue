using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Diagnostics
{
    public class NullLogger : HotLogger
    {
        public NullLogger() : base(null)
        {

        }
    }
}
    