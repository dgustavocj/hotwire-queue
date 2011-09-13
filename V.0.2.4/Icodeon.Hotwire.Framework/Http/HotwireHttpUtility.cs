using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Icodeon.Hotwire.Framework.Security;

namespace Icodeon.Hotwire.Framework.Http
{
    // this class is similar to HttpClientHelper (which has not been checked in to hotwire, as (as of 30/8) still cannot dsitribute the httpClient
    // binary... 
    public static class HotwireHttpUtility
    {
        public static string ToEncodedHttpPostString(this NameValueCollection src)
        {
            // according to wikipedia "Post(http)"
            // http://en.wikipedia.org/wiki/POST_(HTTP)

            // 1. & and =
            // 2. replace all spaces with "+"
            // 3. urlEcoding all other non alphanumeric 

            var sb = new StringBuilder();

            foreach (string key in src.Keys)
            {
                string value = src[key];
                string encodedKey = key.replaceSpacesWithPluses().UrlEncodeAllOtherNonAlphanumeric();
                string encodedValue = value.replaceSpacesWithPluses().UrlEncodeAllOtherNonAlphanumeric();
                sb.AppendFormat("{0}={1}&",encodedKey, encodedValue);
            }
            string joinedString = sb.ToString();
            string postBody = joinedString.TrimEnd('&');
            return postBody;
        }

        public static string Post(NameValueCollection requestParams, Uri uri)
        {
            var body = requestParams.ToEncodedHttpPostString();
            using(var client = new WebClient())
            {
                client.AddFormUrlEncodedHeaders();
                var response = client.Post(uri, body);
                return response;
            }
        }

        public static string Post(this WebClient client, Uri uri, string body)
        {
            var bytes = Encoding.UTF8.GetBytes(body);
            var responseBytes = client.UploadData(uri, "POST", bytes);
            var result = Encoding.UTF8.GetString(responseBytes);
            return result;
        }

        public static WebClient AddFormUrlEncodedHeaders(this WebClient client)
        {
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            return client;
        }

        private static string UrlEncodeAllOtherNonAlphanumeric(this string src)
        {
            return HttpUtility.UrlEncode(src);
        }

        private static string replaceSpacesWithPluses(this string src)
        {
            return src.Replace(' ', '+');
        }
    }
}
