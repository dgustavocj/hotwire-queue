using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.MediaTypes;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public interface IModuleConfiguration
    {
        bool Active { get; set; }
        string RootServiceName { get; set; }
        MethodValidation MethodValidation { get; set; }
        IEnumerable<IModuleEndpoint> Endpoints { get; }
        void AddEndpoint(EndpointConfiguration endpoint);
    }

}
