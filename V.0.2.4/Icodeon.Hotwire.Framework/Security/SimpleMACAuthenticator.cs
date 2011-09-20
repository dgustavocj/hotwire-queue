using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Icodeon.Hotwire.Framework.DAL.Params;
using Icodeon.Hotwire.Framework.Modules;
using Icodeon.Hotwire.Framework.DAL;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Security
{
    // *************************************************************************************************************


    public class SimpleMacAuthenticator : SimpleMacSigner, IAuthenticateRequest
    {
        private readonly ISimpleMacDAL _simpleMacDal;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public SimpleMacAuthenticator(IDateTime dateTimeProvider, ISimpleMacDAL simpleMacDal) : base(dateTimeProvider)
        {
            _simpleMacDal = simpleMacDal;
        }

        public void AuthenticateRequest(NameValueCollection requestParameters,NameValueCollection headers, string httpMethod, EndpointMatch endpointMatch)
        {
            // not validating on user id currently. will add in userId later if/when needed.
            string hotwireMac = GetMacOrThrowException(headers);
            string salt = GetSaltOrThrowException(headers);
            int timeStamp = GetTimeStampOrThrowException(headers);
            int maxAgeSeconds = endpointMatch.Endpoint.TimeStampMaxAgeSeconds ?? 3;
            EnsureTimeStampNotOlderThanMaxAgeSeconds(DateTimeProvider,timeStamp, maxAgeSeconds);
            string url = endpointMatch.Match.RequestUri.ToString();
            string privateKey = endpointMatch.Endpoint.PrivateKey;
            string expectedMac = GenerateMac(privateKey, requestParameters, httpMethod, url, salt,timeStamp);
            if (!hotwireMac.Equals(expectedMac)) throw new InvalidMacUnauthorizedException();
            Guid saltGuid = Guid.Parse(salt);
            EnsureMacAndSaltHaveNotBeenUsedBefore(saltGuid);
            _simpleMacDal.CacheRequest(new CacheRequestParams(saltGuid, url, 1000* maxAgeSeconds));
        }

        private void EnsureMacAndSaltHaveNotBeenUsedBefore(Guid salt)
        {
            bool exists = _simpleMacDal.RequestsExists(salt);
            if (exists) throw new BadRequestMacSaltAlreadyUsedException();
        }

        private void EnsureTimeStampNotOlderThanMaxAgeSeconds(IDateTime dateTime, int timeStamp, int endpointTimeStampMaxAge)
        {
            // allow timestamp to vary by plus or minus the max age, to allow for clock on server and client being either ahead or behind each other.
            if (Math.Abs(timeStamp - dateTime.SecondsSince1970) > endpointTimeStampMaxAge) throw new InvalidMacTimestampExpiredException();
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
