using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using NLog;

namespace Icodeon.Hotwire.Framework
{
    public static class LoggerDiagnosticExtensions
    {


        public static void TraceParameters(this Logger logger, NameValueCollection coll)
        {
            foreach (var key in coll.AllKeys)
            {
                logger.Trace("{0,-30}:{1}",key,coll[key]);
            }
        }

        public static void LogMethodCall(this Logger logger, string methodName, params object[] parameters)
        {
            logger.Trace(methodName);
            logger.Trace("-----------");
            foreach (var parameter in parameters)
            {
                if (parameter == null)
                {
                    logger.Trace("\t{0,-35}{1}", "[nullparameter]", "[NULL]");
                }
                else
                {
                    logger.Trace("\t{0,-35}Not null.", parameter.GetType().ToString());
                }

            }
        }

    }
}
