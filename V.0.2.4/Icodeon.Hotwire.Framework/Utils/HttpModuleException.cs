using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Icodeon.Hotwire.Framework.Diagnostics;
using NLog;

namespace Icodeon.Hotwire.Framework.Utils
{
    //REFACTOR: choose a better name for HttpXXXException so that we're not tied to using this in HttpModules. e.g. HttpHandlers
    public class HttpModuleException : ApplicationException
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public HttpModuleException(HttpStatusCode statusCode, string message, Exception innerException)
            : base(message, innerException)
        {

            StatusCode = statusCode;
        }


        public HttpModuleException(HttpStatusCode statusCode, string message) : base(message)
        {
            _logger.Error("HttpStatusCode=" + statusCode + ". " + message);
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }

    }
}
