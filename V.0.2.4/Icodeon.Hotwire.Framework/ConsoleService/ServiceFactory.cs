using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using Icodeon.Hotwire.ConsoleService;
using Icodeon.Hotwire.Framework.ConsoleService;
using framework = Icodeon.Hotwire.Framework;
using NLog;

namespace Icodeon.Hotwire.Tests.Acceptance.Internal
{
    public static class ServiceFactory
    {
        public static ServiceContextDTO GivenARunningSelfHostedHotwireService()
        {
            var hotwire = new framework.HotwireService();
            var config = ConsoleServiceSection.ReadConfig();
            var uri = config.GetUri(Environment.MachineName);
            var host = new WebServiceHost(hotwire, uri);
            host.Open(); 
            return new ServiceContextDTO
                       {
                           Host = host,
                           HotwireSingleton = hotwire,
                           Uri = uri
                       };
        }

    }
}
