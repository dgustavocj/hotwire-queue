using System;
using System.Collections.Specialized;

namespace Icodeon.Hotwire.Contracts
{
    public interface IOAuthProvider 
    {
        NameValueCollection GenerateSignedParametersForPost(string consumerKey, string consumerSecret, Uri rawUri, NameValueCollection nonOAuthParams);
        bool IsValidSignatureForPost(string consumerKey, string consumerSecret, Uri uri, NameValueCollection form);
    }
}
