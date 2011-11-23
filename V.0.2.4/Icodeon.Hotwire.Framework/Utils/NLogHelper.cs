using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework
{
    public static class NLogHelper
    {

        /// <summary>
        /// Format string should take 1 or 2 parameters e.g. "Name={0},Value={1}" or "{0}={1}", if only 1 parameter is supplied then the value is used.
        /// </summary>
        public static void LogTraceNewLinePerItem(this NLog.Logger logger,NameValueCollection nv, string formatString)
        {
            foreach(string key in nv.AllKeys)
            {
                logger.Trace(formatString,key,nv[key]);
            }
        }


        public static void LogTraceNewLinePerItem(this NLog.Logger logger, NameValueCollection nv)
        {
            int width = nv.AllKeys.Max(k => k.Length) + 1;
            string format = "\t{0,-width}:{1}".Replace("width", width.ToString());
            logger.LogTraceNewLinePerItem(nv,format);
        }
    }
}
