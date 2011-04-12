using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Icodeon.Hotwire.Contracts;


namespace Icodeon.Hotwire.Framework.Security
{
    public class QuickAuth
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly IOAuthProvider _oAuth;

        public QuickAuth(string consumerKey, string consumerSecret, IOAuthProvider oAuth)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _oAuth = oAuth;
        }

        public string DoPostGetResponse(Uri uri)
        {
            return DoPostGetResponse(new NameValueCollection(), uri);
        }

        public string DoPostGetResponse(NameValueCollection nonOAuthParams, Uri uri)
        {

            NameValueCollection param = _oAuth.GenerateSignedParametersForPost(_consumerKey, _consumerSecret, uri, nonOAuthParams);
            string postBody="";
            foreach (string key in param.Keys)
            {
                postBody += key + "=" + param[key] + "&";
            }
            postBody = postBody.Trim('&');

            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            byte[] postBuffer = Encoding.UTF8.GetBytes(postBody);
            byte[] responseBuffer = webClient.UploadData(uri, "POST", postBuffer);
            string result = Encoding.UTF8.GetString(responseBuffer);
            return result;
        }

        public string DoNonOauthPostGetResponse(NameValueCollection bodyParameters, Uri uri)
        {
            string postBody = "";
            foreach (string key in bodyParameters.Keys)
            {
                postBody += key + "=" + HttpUtility.UrlEncode(bodyParameters[key]) + "&";
            }
            postBody = postBody.Trim('&');

            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            byte[] byteArray = Encoding.ASCII.GetBytes(postBody);
            byte[] responseArray = webClient.UploadData(uri, "POST", byteArray);
            string result = Encoding.UTF8.GetString(responseArray);
            return result;
        }

        public bool ValidateSignature(Uri requestUri, NameValueCollection bodyParameters)
        {
            bool validationResult = _oAuth.IsValidSignatureForPost(_consumerKey, _consumerSecret, requestUri, bodyParameters);
            return validationResult;
        }


    }

}
