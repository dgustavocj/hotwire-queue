using System;
using System.ServiceModel.Web;
using Icodeon.Hotwire.Framework;

namespace Icodeon.Hotwire.Framework.ConsoleService
{
    public class ServiceContextDTO 
    {
        public Uri Uri { get; set; }
        public WebServiceHost Host { get; set; }
        public HotwireService HotwireSingleton { get; set;  }
    }
}