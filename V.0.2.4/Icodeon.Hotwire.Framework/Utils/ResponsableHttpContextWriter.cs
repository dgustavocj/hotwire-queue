using System.Web;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Utils
{
    public class ResponsableHttpContextWriter : IHttpResponsableWriter
    {
        private HttpResponse _response;

        public ResponsableHttpContextWriter(HttpResponse response)
        {
            _response = response;
        }

        public string ContentType
        {
            get { return _response.ContentType; }
            set { _response.ContentType = value; }
        }

        public void Write(string s)
        {
            _response.Write(s);
        }

        public int StatusCode
        {
            get { return _response.StatusCode; }
            set { _response.StatusCode = value; }
        }
    }
}