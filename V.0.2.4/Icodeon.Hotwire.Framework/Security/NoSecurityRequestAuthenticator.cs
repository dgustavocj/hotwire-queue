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


        public void SignRequestAddToHeaders(NameValueCollection headers, string privateKey, string httpMethod, Uri uri, string macSalt)
        {
            throw new NotImplementedException();
        }

        public void SignRequestAddToHeaders(NameValueCollection headers, string privateKey, NameValueCollection requestParameters, string httpMethod, Uri uri, string macSalt)
        {
            throw new NotImplementedException();
        }
    }
}
