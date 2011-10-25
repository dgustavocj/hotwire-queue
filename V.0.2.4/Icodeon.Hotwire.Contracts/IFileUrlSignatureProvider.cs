using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Contracts
{
    public interface IFileUrlSignatureProvider
    {
        Uri SignUrl(
            Uri urlToSign,
            NameValueCollection requestParams);
    }
}
