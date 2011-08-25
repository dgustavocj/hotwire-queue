using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Modules;

namespace Icodeon.Hotwire.Framework.Security
{
    public class SimpleMACAuthenticator : IAuthenticateRequest 
    {

        public void AuthenticateRequest(NameValueCollection requestParameters, string httpMethod, EndpointMatch endpointMatch)
        {
            throw new NotImplementedException();
        }
    }
}
