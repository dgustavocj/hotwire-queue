using System;
using System.IO;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Modules;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class MockStreamingContext : StreamingContext
    {
        public bool HasCompleted { get; set; }

        public MockStreamingContext(string requestUri, IModuleConfiguration configuration): this(null, requestUri,configuration)
        {
            
        }

        public MockStreamingContext(string streamContent, string requestUri, IModuleConfiguration configuration)
        {
            ApplicationCache = new MockAppCache();
            HttpMethod = "GET";
            GetLogger = () => HotLogger.NullLogger;
            HttpWriter = new MockHttpWriter();
            PathMapper = new MockMapPath();
            InputStream = new MemoryStream();
            new StreamWriter(InputStream).Write(streamContent ?? "");
            CompleteRequest = () => { HasCompleted = true; };
            Configuration = configuration;
            Url = new Uri(requestUri);
            UserHostAddress = "localhost";
        }

    }
}
