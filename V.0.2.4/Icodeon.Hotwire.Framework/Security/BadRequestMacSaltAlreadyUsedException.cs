using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Security
{
    public class BadRequestMacSaltAlreadyUsedException : HttpModuleException
    {
        public BadRequestMacSaltAlreadyUsedException() : base(HttpStatusCode.BadRequest, "Salt and-or Mac have already been used.") { }
    }
}
