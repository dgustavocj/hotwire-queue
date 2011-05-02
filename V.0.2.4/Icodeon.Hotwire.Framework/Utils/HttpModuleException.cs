using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Icodeon.Hotwire.Framework.Diagnostics;
using NLog;

namespace Icodeon.Hotwire.Framework.Utils
{
    public class HttpModuleException : ApplicationException
    {

        public HttpModuleException(HttpStatusCode statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }

        public HttpModuleException(LoggerBase logger, HttpStatusCode statusCode, string message)
            : this(statusCode, message)
        {
            logger.Error("HttpStatusCode=" + statusCode + ". " + message);
        }

        public HttpModuleException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }

    }
}
