using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Contracts
{
    public interface IOAuthProvider
    {
        NameValueCollection GenerateSignedParametersForPost(string consumerKey, string consumerSecret, Uri rawUri, NameValueCollection nonOAuthParams);
        bool IsValidSignatureForPost(string consumerKey, string consumerSecret, Uri uri, NameValueCollection form);
    }
}
