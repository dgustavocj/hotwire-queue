using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Modules
{
    public class ParsedContext
    {
        public IAppCache AppCache { get; set; }
        public NameValueCollection RequestParameters { get; set; }
        public Uri Url { get; set; }
        public UriTemplateMatch Match { get; set; }
        public IModuleEndpoint ModuleConfig { get; set; }
        public IMediaInfo MediaInfo { get; set; }
        public IMapPath PathMapper { get; set; }
    }
}
