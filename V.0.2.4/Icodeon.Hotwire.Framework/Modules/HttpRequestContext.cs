using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Configuration;

namespace Icodeon.Hotwire.Framework.Modules
{
    public class HttpRequestContext
    {
        public IModuleConfiguration Configuration { get; set; }
        public Uri Url { get; set; }
        public string UserHostAddress { get; set; }
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(string.Format("Url:{0}, UserHostAddress:{1}",Url, UserHostAddress));
            return sb.ToString();
        }

    }
}
