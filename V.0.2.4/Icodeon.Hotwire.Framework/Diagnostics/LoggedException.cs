using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Icodeon.Hotwire.Framework.Diagnostics
{
    public class LoggedException : Exception
    {
        public LoggedException(Logger logger, string message) : base(message)
        {
            logger.Fatal(message);
        }
    }
}
