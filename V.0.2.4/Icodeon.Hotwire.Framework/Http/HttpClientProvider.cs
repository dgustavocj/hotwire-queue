using System;
using System.Net.Http;
using Icodeon.Hotwire.Contracts;

namespace Icodeon.Hotwire.Framework.Http
{
    public class HttpClientProvider : IHttpClientProvider
    {

        public HttpClientProvider()
        {

        }

        public void GetAndEnsureStatusIsSuccessful(Uri uri)
        {
            var client = new HttpClient();
            var response = client.Get(uri);
            response.EnsureSuccessStatusCode();
        }



        public string GetResponseAsStringEnsureStatusIsSuccessful(Uri uri)
        {
            var client = new HttpClient();
            var response = client.Get(uri);
            response.EnsureSuccessStatusCode();
            string responsetext = response.Content.ReadAsString();
            return responsetext;
        }
    }
}
