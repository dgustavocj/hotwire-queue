using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Scripts
{
    public class GetResultDTO
    {
        public int MilliSeconds { get; set; }
        public string ResponseText { get; set; }
        public int StatusCode { get; set; }
        public string ContentType { get; set; }
    }
}
