using System;
using System.Collections.Specialized;
using System.IO;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Modules
{
    public class StreamingContext : HttpRequestContext
    {
        public IAppCache ApplicationCache { get; set; }
        public string HttpMethod { get; set; }
        public IHttpResponsableWriter HttpWriter { get; set; }
        public IMapPath PathMapper { get; set; }
        public Stream InputStream { get; set; }
        public Action CompleteRequest { get; set; }
        public NameValueCollection Headers { get; set; }

    }
}