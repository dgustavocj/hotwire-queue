using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.ConsoleService
{

    public class ConsoleServiceSection : ConfigurationSection, IConsoleServiceConfig
    {
        [ConfigurationProperty("port", DefaultValue = 8133, IsRequired = true)]
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("uriTemplate", DefaultValue  = "http://{machine-name}:{port}/hotwire/", IsRequired = true)]
        public string UriTemplate
        {
            get { return (string)this["uriTemplate"]; }
            set
            {
                if (!value.Contains("{machine-name}")) throw new ArgumentException("{machine-name} token missing from uriTemplate value.");
                if (!value.Contains("{port}")) throw new ArgumentException("{port} token missing from uriTemplate value.");
                this["uriTemplate"] = value;
            }
        }

        public const string TokenMachineName = "{machine-name}";
        public const string TokenPort = "{port}";

        public Uri Uri()
        {
            string UriString = UriTemplate.Replace(TokenMachineName, Environment.MachineName).Replace(TokenPort, Port.ToString());
            return new Uri(UriString);
        }

        // todo: replace with config section reader (factory)
        public static IConsoleServiceConfig ReadConfig()
        {
            return (IConsoleServiceConfig)ConfigurationManager.GetSection( Constants.Configuration.SectionGroup +@"/consoleService");
        }

    } // class
} // namespace 