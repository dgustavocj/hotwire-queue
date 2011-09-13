using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Icodeon.Hotwire.Framework.Security
{
    public class InvalidMacUnauthorizedException : HttpUnauthorizedException
    {
        public InvalidMacUnauthorizedException(string message) : base("SimpleMAC authentication is configured for this request. "+ message) { }
        public InvalidMacUnauthorizedException() : this("No valid MAC was found in the headers.") { }
    }
}
