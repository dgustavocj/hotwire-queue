using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Icodeon.Hotwire.Framework.MediaTypes
{

    [DataContract]
    public enum eMediaType
    {
        [EnumMember]
        json,
        [EnumMember]
        xml,
        [EnumMember]
        text,
        [EnumMember]
        html
    }
}
