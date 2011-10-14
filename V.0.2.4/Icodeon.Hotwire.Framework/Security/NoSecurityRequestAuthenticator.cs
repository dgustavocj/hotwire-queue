using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Security
{
    public class NoSecurityRequestAuthenticator : IAuthenticateRequest 
    {

        public void AuthenticateRequest(NameValueCollection requestParameters,NameValueCollection headers,  string httpMethod, Modules.EndpointMatch endpointMatch)
        {
            // do nothing. (for now).
            return;
        }
    }
}
