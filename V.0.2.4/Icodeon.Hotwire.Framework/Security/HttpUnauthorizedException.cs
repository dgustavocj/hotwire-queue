using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Security
{
    public class HttpUnauthorizedException : HttpModuleException
    {
        public HttpUnauthorizedException(string message) : base(HttpStatusCode.Unauthorized, message) { }
    }
}
