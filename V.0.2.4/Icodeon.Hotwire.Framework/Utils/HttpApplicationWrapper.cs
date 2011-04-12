using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Icodeon.Hotwire.Framework.Utils
{
    public class HttpApplicationWrapper : IMapPath
    {
        private readonly HttpApplication _application;

        public HttpApplicationWrapper(HttpApplication application)
        {
            _application = application;
        }

        public string MapPath(string path)
        {
            return _application.Server.MapPath(path);
        }
    }
}
