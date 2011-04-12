using System;
using System.ServiceModel.Web;
using framework = Icodeon.Hotwire.Framework;

namespace Icodeon.Hotwire.Framework.ConsoleService
{
    public static class ConsoleServiceFactory
    {
        public static ServiceContextDTO GivenARunningSelfHostedHotwireService(IConsoleServiceConfig config)
        {
            var hotwire = new HotwireService();
            var uri = config.Uri();
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
