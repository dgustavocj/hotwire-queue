﻿using System;
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
        // getting a logger can be slow due to reflection, so we don't want to force evaluation of this
        // assuming that if the "Uri match" is null then we won't be processing anything.
        public Func<LoggerBase> GetLogger { get; set; }
        public IHttpResponsableWriter HttpWriter { get; set; }
        public IMapPath PathMapper { get; set; }
        public Stream InputStream { get; set; }
        public Action CompleteRequest { get; set; }
    }
}