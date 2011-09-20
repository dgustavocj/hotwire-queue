using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.DAL
{
    public static class ConnectionStringManager
    {
        public static string HotwireConnectionString
        {
            get
            {
                var cStringConfig = ConfigurationManager.ConnectionStrings["hotwire"];
                if (cStringConfig == null) throw new ArgumentNullException("Could not find connection string setting 'hotwire'");
                return cStringConfig.ConnectionString;
            }
        }

    }
}
