using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using NLog;

namespace Icodeon.Hotwire.Framework.Diagnostics
{
    public static class LoggerExtensions
    {
        public static void TraceParameters(this Logger logger, NameValueCollection coll)
        {
            foreach (var key in coll.AllKeys)
            {
                logger.Trace("{0,-30}:{1}", key, coll[key]);
            }
        }

    }
}
