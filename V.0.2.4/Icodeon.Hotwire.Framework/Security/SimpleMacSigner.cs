using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Security
{
    public class SimpleMacSigner : ISignRequest
    {
        protected readonly IDateTime DateTimeProvider;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public SimpleMacSigner(IDateTime dateTimeProvider)
        {
            DateTimeProvider = dateTimeProvider;
        }

        private StringBuilder ToHashableStringBuilder(NameValueCollection requestParameters)
        {
            StringBuilder sb = new StringBuilder();
            if (requestParameters == null) return sb;
            foreach (var requestParameter in requestParameters)
            {
                // don't need carriage returns or any line terminations
                sb.Append("{0}{1}");
            }
            return sb;
        }


        //public void SignRequestAddToHeaders(NameValueCollection headers, string privateKey, string httpMethod, Uri uri, string macSalt, int timeStamp)
        //{
        //    SignRequestAddToHeaders(headers,privateKey,null, httpMethod, uri, macSalt,timeStamp);
        //}

        public void SignRequestAddToHeaders(NameValueCollection headers, string privateKey, NameValueCollection requestParameters, string httpMethod, Uri uri, string macSalt, int timeStamp)
        {
            if (string.IsNullOrEmpty(macSalt)) throw new ArgumentOutOfRangeException("macSalt", "cannot be null.");
            string mac = GenerateMac(privateKey, requestParameters, httpMethod, uri.ToString(), macSalt, timeStamp);
            // OAuth allows you to place these values into body, url, or headers 
            headers.Add(SimpleMACHeaders.HotwireMacHeaderKey, mac);
            headers.Add(SimpleMACHeaders.HotwireMacSaltHeaderKey, macSalt);
            headers.Add(SimpleMACHeaders.HotwireMacTimeStampKey, timeStamp.ToString());
        }


        public string CalculateMac(string privateKey, NameValueCollection requestParameters, string httpMethod, Uri uri, string macSalt, int timeStamp)
        {
            string mac = GenerateMac(privateKey, requestParameters, httpMethod, uri.ToString(), macSalt, timeStamp);
            return mac;
        }


        private HMAC CreateHmac(byte[] bytes)
        {
            return new HMACSHA256(bytes);
        }


        private string GetHexString(byte[] macBytes)
        {
            StringBuilder sb = new StringBuilder();
            macBytes.ToList().ForEach(b => sb.AppendFormat("{0:x}", (object) b));
            return sb.ToString();
        }



        // see http://chargen.matasano.com/chargen/2007/9/7/enough-with-the-rainbow-tables-what-you-need-to-know-about-s.html
        // if you need something super secure or more secure that our implementation.


        protected string GenerateMac(string privateKey, NameValueCollection requestParameters, string httpMethod, string url, string macSalt, int timeStamp)
        {
            try
            {
                var utf8Encoder = new UTF8Encoding();
                byte[] keyBytes = utf8Encoder.GetBytes(privateKey);
                using (HMAC generator = CreateHmac(keyBytes))
                {
                    var hashableString = ToHashableStringBuilder(requestParameters)
                        .Append(httpMethod)
                        .Append(url)
                        .Append(macSalt)
                        .Append(timeStamp)
                        .ToString();
                    var hashableBytes = utf8Encoder.GetBytes(hashableString);
                    byte[] macBytes = generator.ComputeHash(hashableBytes);
                    string hashString = GetHexString(macBytes);
                    return hashString;
                }
            }
            catch (Exception ex)
            {
                var httpex = new HttpUnauthorizedException("Unexpected exception attempting to validate request MAC. Exception details have been logged.");
                _logger.LogException(LogLevel.Error, httpex.Message, ex);
                throw httpex;
            }
        }


    }
}