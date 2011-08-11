using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.MediaTypes;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public interface IModuleEndpoint
    {
        UriTemplate UriTemplate { get; set; }
        IEnumerable<string> HttpMethods { get; set; }
        eMediaType MediaType { get; set; }
        string Action { get; set; }
        bool Active { get; set; }
        string Name { get; set; }
        SecurityType Security { get; set; }
    }
}
