using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class EndpointCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EndpointConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EndpointConfiguration)element).UriTemplate;
        }
    }
}
