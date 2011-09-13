using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Modules;

namespace Icodeon.Hotwire.Framework.Security
{
    public interface IAuthenticateRequest
    {
        void AuthenticateRequest(NameValueCollection requestParameters, NameValueCollection headers, string httpMethod, EndpointMatch endpointMatch);
    }
}
