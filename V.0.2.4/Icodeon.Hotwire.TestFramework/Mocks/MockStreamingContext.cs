using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Http;
using Icodeon.Hotwire.Framework.Modules;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class MockStreamingContext : StreamingContext
    {
        public MockHttpWriter MockWriter { get; set; }
        public bool HasCompleted { get; set; }

        public MockStreamingContext(NameValueCollection requestParameters, string requestUri, IModuleConfiguration configuration) :
            this(requestParameters.ToEncodedHttpPostString(), requestUri, configuration)
        {
           
        }

        public MockStreamingContext(string requestUri, IModuleConfiguration configuration): this(string.Empty, requestUri,configuration)
        {
            
        }

        public MockStreamingContext(string streamContent, string requestUri, IModuleConfiguration configuration)
        {
            ApplicationCache = new MockAppCache();
            HttpMethod = "GET";
            MockWriter = new MockHttpWriter();
            HttpWriter = MockWriter;
            PathMapper = new MockMapPath();
            PrepareStream(streamContent);
            CompleteRequest = () => { HasCompleted = true; };
            Configuration = configuration;
            Url = new Uri(requestUri);
            UserHostAddress = "localhost";
            Headers = new NameValueCollection();
        }

        private void PrepareStream(string streamContent)
        {
            InputStream = new MemoryStream();
            var streamWriter = new StreamWriter(InputStream);
            streamWriter.Write(streamContent ?? "");
            streamWriter.Flush();
            InputStream.Position = 0;
        }
    }
}
