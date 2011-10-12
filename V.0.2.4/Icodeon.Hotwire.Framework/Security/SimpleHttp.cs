using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;

namespace Icodeon.Hotwire.Framework.Security
{
    public static class SimpleHttp
    {
        public static string DoFormUrlEncodedPostGetResponse(NameValueCollection parameters, Uri uri)
        {
            //TODO: do I need to change this to use my (Hotwire's) stricter http encoder?
            string postBody="";
            foreach (string key in parameters.Keys)
            {
                postBody += key + "=" + parameters[key] + "&";
            }
            postBody = postBody.Trim('&');

            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            byte[] postBuffer = Encoding.UTF8.GetBytes(postBody);
            byte[] responseBuffer = webClient.UploadData(uri, "POST", postBuffer);
            string result = Encoding.UTF8.GetString(responseBuffer);
            return result;
        }
    }
}
