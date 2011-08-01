using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Security
{
    public interface IAuthenticateRequest
    {
        void AuthenticateRequest(NameValueCollection queueParameters);
    }
}
