using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Security
{
    public class InvalidMacTimestampExpiredException : InvalidMacUnauthorizedException
    {
        public InvalidMacTimestampExpiredException() : base("TimeStamp expired."){}
    }
}
