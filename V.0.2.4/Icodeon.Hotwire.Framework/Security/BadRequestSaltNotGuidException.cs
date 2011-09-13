using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Security
{
    public class BadRequestSaltNotGuidException : HttpModuleException
    {
        public BadRequestSaltNotGuidException() : base(HttpStatusCode.BadRequest,"Salt is not a guid.") {}
    }
}
