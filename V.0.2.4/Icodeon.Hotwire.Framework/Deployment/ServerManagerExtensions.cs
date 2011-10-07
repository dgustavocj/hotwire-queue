using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;

namespace Icodeon.Hotwire.Framework.Deployment
{
    public static class ServerManagerExtensions
    {
        public static string DomainNameList(this ServerManager manager)
        {
            string domainNames = string.Join(",", manager.Sites.Select(s => s.Name + "[" + s.State + "]" + s.Bindings[0].ToString()));
            return domainNames;
        }


    }
}
