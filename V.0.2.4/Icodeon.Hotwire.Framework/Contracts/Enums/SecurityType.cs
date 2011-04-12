using System.Runtime.Serialization;

namespace Icodeon.Hotwire.Framework.Contracts
{
    [DataContract]
    public enum SecurityType
    {
        [EnumMember]
        none,

        /// <summary>
        /// currently using oauth version 1.0, with HMAC-SHA1 signature method
        /// </summary>
        [EnumMember]
        oauth,

        [EnumMember]
        localonly
    }
}