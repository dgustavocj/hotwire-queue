using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Modules;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Security
{
    public interface IAuthenticateRequest
    {
        void AuthenticateRequest(BodyParsed body, NameValueCollection headers, string httpMethod, EndpointMatch endpointMatch);
    }
}
