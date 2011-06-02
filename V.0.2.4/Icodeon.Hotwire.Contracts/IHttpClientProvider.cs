using System;

namespace Icodeon.Hotwire.Contracts
{
    public interface IHttpClientProvider 
    {
        void GetAndEnsureStatusIsSuccessful(Uri uri);
        string GetResponseAsStringEnsureStatusIsSuccessful(Uri uri);
    }
}
