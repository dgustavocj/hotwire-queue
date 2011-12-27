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
            client.GetAsync(uri).ContinueWith(t =>
                              t.Result.EnsureSuccessStatusCode()
                              );

        }



        public string GetResponseAsStringEnsureStatusIsSuccessful(Uri uri)
        {
            var client = new HttpClient();
            string responseText="returned before assigned, need 'await' or some type of thread.join?";
            client.GetAsync(uri).ContinueWith(t =>{
                        t.Result.EnsureSuccessStatusCode();
                        responseText = t.Result.Content.ToString();
                    });
            return responseText;
        }
    }
}
