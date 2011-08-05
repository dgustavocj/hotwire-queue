using System.Configuration;
using Icodeon.Hotwire.Framework.Contracts;


namespace Icodeon.Hotwire.TestAspNet
{
    public class SmokeTestsConfig
    {
        private string _othersite;

        public SmokeTestsConfig()
        {
            _othersite = ConfigurationManager.AppSettings["othersite"];
        }

        public string BaseUri
        {
            get { return string.Format("http://{0}", _othersite); }
        }

        public string EchoTestUri
        {
            get { return string.Format("http://{0}{1}", _othersite, Uris.Ver0_2.HotwireTests.HotwireTests_Echo); }
        }

        public string FileMoveTestUri
        {
            get { return string.Format("http://{0}{1}", _othersite, Uris.Ver0_2.HotwireTests.HotwireTests_FileMove); }
        }

        public string FileMoveBackgroundThreadTestUriHtml
        {
            get { return string.Format("http://{0}{1}.html", _othersite, Uris.Ver0_2.HotwireTests.HotwireTests_BackgroundFileMove); }
        }


    } // class
} // namespace 