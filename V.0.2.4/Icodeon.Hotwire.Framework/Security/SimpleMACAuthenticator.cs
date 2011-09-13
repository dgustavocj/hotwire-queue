﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Icodeon.Hotwire.Framework.Modules;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Security
{
    public class SimpleMacAuthenticator : IAuthenticateRequest, ISignRequest
    {
        private readonly IDateTime _dateTimeProvider;
        private Logger _logger;


        public SimpleMacAuthenticator(IDateTime dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            _logger = LogManager.GetLogger("Icodeon.Hotwire.Framework.Security.SimpleMACAuthenticator");
        }

        //TODO: Encrypt the expiry time into the nonce (salt)

        public void AuthenticateRequest(NameValueCollection requestParameters,NameValueCollection headers, string httpMethod, EndpointMatch endpointMatch)
        {
            string hotwireMac = GetMacOrThrowException(headers);
            string nonce = GetSaltOrThrowException(headers);
            int timeStamp = GetTimeStampOrThrowException(headers);
            EnsureTimeStampNotOlderThanMaxAgeSeconds(_dateTimeProvider,timeStamp, endpointMatch.Endpoint.TimeStampMaxAgeSeconds ?? 3);
            string url = endpointMatch.Match.RequestUri.ToString();
            string privateKey = endpointMatch.Endpoint.PrivateKey;
            string expectedMac = GenerateMd5Mac(privateKey, requestParameters, httpMethod, url, nonce,timeStamp);
            if (!hotwireMac.Equals(expectedMac)) throw new InvalidMacUnauthorizedException();
        }

        private void EnsureTimeStampNotOlderThanMaxAgeSeconds(IDateTime dateTime, int timeStamp, int endpointTimeStampMaxAge)
        {
            // allow timestamp to vary by plus or minus the max age, to allow for clock on server and client being either ahead or behind each other.
            if (Math.Abs(timeStamp - dateTime.SecondsSince1970) > endpointTimeStampMaxAge) throw new InvalidMacTimestampExpiredException();
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


        public void SignRequestAddToHeaders(NameValueCollection headers, string privateKey, string httpMethod, Uri uri, string macSalt, int timeStamp)
        {
            SignRequestAddToHeaders(headers,privateKey,null, httpMethod, uri, macSalt,timeStamp);
        }

        public void SignRequestAddToHeaders(NameValueCollection headers, string privateKey, NameValueCollection requestParameters, string httpMethod, Uri uri, string macSalt, int timeStamp)
        {
            if (string.IsNullOrEmpty(macSalt)) throw new ArgumentOutOfRangeException("macSalt", "cannot be null.");
            string mac = GenerateMd5Mac(privateKey, requestParameters, httpMethod, uri.ToString(), macSalt, timeStamp);
            // OAuth allows you to place these values into body, url, or headers 
            headers.Add(SimpleMACHeaders.HotwireMacHeaderKey, mac);
            headers.Add(SimpleMACHeaders.HotwireMacSaltHeaderKey, macSalt);
            headers.Add(SimpleMACHeaders.HotwireMacTimeStampKey, timeStamp.ToString());
        }

        // this is weak encryption, but good enough for our own use, in the case where we will secure access to endpoint via IP address restriction
        // see http://chargen.matasano.com/chargen/2007/9/7/enough-with-the-rainbow-tables-what-you-need-to-know-about-s.html
        // the reason for simpleMAC authorization is for the cases where firewall and/or other security is accidentally turned off.

        public string CalculateMac(string privateKey, NameValueCollection requestParameters, string httpMethod, Uri uri, string macSalt, int timeStamp)
        {
            string mac = GenerateMd5Mac(privateKey, requestParameters, httpMethod, uri.ToString(), macSalt, timeStamp);
            return mac;
        }



        private string GenerateMd5Mac(string privateKey, NameValueCollection requestParameters, string httpMethod, string url, string macSalt, int timeStamp)
        {
            try
            {
                var utf8Encoder = new UTF8Encoding();
                byte[] keyBytes = utf8Encoder.GetBytes(privateKey);
                using (HMACMD5 md5MacGenerator = new HMACMD5(keyBytes))
                {
                    var hashableString = ToHashableStringBuilder(requestParameters)
                        .Append(httpMethod)
                        .Append(url)
                        .Append(macSalt)
                        .Append(timeStamp)
                        .ToString();
                    var hashableBytes = utf8Encoder.GetBytes(hashableString);
                    byte[] md5MacBytes = md5MacGenerator.ComputeHash(hashableBytes);
                    string md5HashString = GetHexString(md5MacBytes);
                    return md5HashString;
                }
            }
            catch (Exception ex)
            {
                var httpex = new HttpUnauthorizedException("Unexpected exception attempting to validate request MAC. Exception details have been logged.");
                _logger.LogException(LogLevel.Error, httpex.Message, ex);
                throw httpex;
            }
        }

        private string GetHexString(byte[] md5MacBytes)
        {
            StringBuilder sb = new StringBuilder();
            md5MacBytes.ToList().ForEach(b => sb.AppendFormat("{0:x}",b));
            return sb.ToString();
        }

        private int GetTimeStampOrThrowException(NameValueCollection headers)
        {
            string timeStamp  = headers.GetValueOrDefault(SimpleMACHeaders.HotwireMacTimeStampKey);
            if (timeStamp == null) throw new BadRequestMissingTimestampException();
            int time;
            const int secondsFrom1970Till2000 = 946080000;
            if (!int.TryParse(timeStamp, out time) || time <= secondsFrom1970Till2000) throw new InvalidMacTimestampExpiredException();
            return time;
        }

        private string GetMacOrThrowException(NameValueCollection headers)
        {
            string mac = headers.GetValueOrDefault(SimpleMACHeaders.HotwireMacHeaderKey);
            if (mac == null) throw new InvalidMacUnauthorizedException();
            return mac;
        }

        private string GetSaltOrThrowException(NameValueCollection headers)
        {
            string salt = headers.GetValueOrDefault(SimpleMACHeaders.HotwireMacSaltHeaderKey);
            if (salt == null) throw new InvalidMacUnauthorizedException("No '" + SimpleMACHeaders.HotwireMacSaltHeaderKey + "' found.");
            if (!salt.IsGuid()) throw new BadRequestSaltNotGuidException();
            return salt;
        }

    }
}
