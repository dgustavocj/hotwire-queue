using System;
using System.Collections.Specialized;
using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.DTOs;
using Icodeon.Hotwire.Framework.MediaTypes;
using Icodeon.Hotwire.Framework.Modules;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class MockParsedContext : ParsedContext
    {
        public MockParsedContext(string action)
        {
            AppCache = new MockAppCache();
            Logger = HotLogger.NullLogger;
            PathMapper = new MockMapPath();
            RequestParameters = new NameValueCollection();
            Url = new Uri(@"http://localhost:1234/doesnotexist");
            Match = new UriTemplateMatch();
            MediaInfo = new MediaTypeFactory().Text;
            ModuleConfig = new EndpointDTO();
            ModuleConfig.Action = action;
        }
    }
}