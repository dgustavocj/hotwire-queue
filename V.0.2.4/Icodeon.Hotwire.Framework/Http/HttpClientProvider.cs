using System;
using System.Net.Http;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Threading;

namespace Icodeon.Hotwire.Framework.Http
{
    public class HttpClientProvider : IHttpClientProvider
    {

        // awefully hacky sychronous "Get" implementation!
        public void GetAndEnsureStatusIsSuccessful(Uri uri)
        {
            new HttpClient().GetAsync(uri).ToNonscalingSync(t=> t.EnsureSuccessStatusCode());
        }


        public string GetResponseAsStringEnsureStatusIsSuccessful(Uri uri)
        {
            var client = new HttpClient();
            string responseText = null;
            client.GetAsync(uri).ToNonscalingSync(t =>{
                                                          t.EnsureSuccessStatusCode();
                                                          TaskExtensions.ToNonscalingSync(t.Content.ReadAsStringAsync(), r => { responseText = r; }, 10000);
                                                      }, 10000);
            return responseText;
        }
    }
}
