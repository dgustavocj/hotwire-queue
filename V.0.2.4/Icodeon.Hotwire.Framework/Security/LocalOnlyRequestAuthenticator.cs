using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Security
{
    public class LocalOnlyRequestAuthenticator : IAuthenticateRequest 
    {

        public void AuthenticateRequest(System.Collections.Specialized.NameValueCollection queueParameters)
        {
            throw new NotImplementedException();
        }
    }
}
