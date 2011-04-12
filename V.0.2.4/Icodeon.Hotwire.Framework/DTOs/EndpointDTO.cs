using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.DTOs
{
    //TODO: Move to configuration namespace
    public class EndpointDTO : IModuleEndpoint
    {

        public UriTemplate UriTemplate { get; set; }

        public IEnumerable<string> HttpMethods { get; set; }

        public MediaTypes.eMediaType MediaType { get; set; }

        public string Action { get; set; }

        public bool Active { get; set; }

        public string Name { get; set; }

        public SecurityType Security { get; set; }
    }
}
