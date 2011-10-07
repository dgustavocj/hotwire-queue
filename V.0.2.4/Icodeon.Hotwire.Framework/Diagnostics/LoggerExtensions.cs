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

        public static void LoggedExecution(this Logger logger, Action code)
        {
            try
            {
                code();
            }
            catch (Exception ex)
            {
                // don't need to re-log Logged exception, as it's already been logged!
                if (ex is LoggedException) throw;
                logger.LogException(LogLevel.Error,ex.ToString(), ex);
                throw;
            }
        }

        public static void TraceParameters(this Logger logger, NameValueCollection coll)
        {
            foreach (var key in coll.AllKeys)
            {
                logger.Trace("{0,-30}:{1}", key, coll[key]);
            }
        }

    }
}
