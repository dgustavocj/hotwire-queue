using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Security
{
    public class SimpleMACHeaders
    {
        public const string HotwireMacHeaderKey = "HotwireMac";
        public const string HotwireMacSaltHeaderKey = "HotwireMacSalt";
        public const string HotwireMacTimeStampKey = "HotwireMacTimeStamp";
        //public const string HotwireMacUserIdKey = "HotwireMacUserIdKey";
    }
}

//<simpleMac maxTimeStampAgeSeconds="10">
//<users>
//    <user id="OU"         privateKey="fsdfsd"/>
//    <user id="Aquella"    privateKey="fsdfsd"/>
//    <user id="test"       privateKey="fsdfsd"/> 
//    <user id="dev1"       privateKey="fsdfsd"/> 
//</users>
//</simpleMac>
