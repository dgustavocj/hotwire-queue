using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Modules;

namespace Icodeon.Hotwire.Framework.Security
{
    public class LocalOnlyRequestAuthenticator : IAuthenticateRequest 
    {
        public void AuthenticateRequest(NameValueCollection requestParameters, NameValueCollection headers, string httpMethod, EndpointMatch endpointMatch)
        {
            throw new NotImplementedException();
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
