using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Security
{
    public interface ISignRequest
    {
        string CalculateMac(string privateKey, NameValueCollection requestParameters, string httpMethod, Uri uri, string macSalt, int timeStamp);
    }
}
