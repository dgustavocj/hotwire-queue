using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Configuration;

namespace Icodeon.Hotwire.Framework.DTOs
{
    public class ModuleConfigurationDTO : IModuleConfiguration
    {
        public List<IModuleEndpoint> _endpoints;

        public ModuleConfigurationDTO()
        {
            _endpoints = new List<IModuleEndpoint>();
        }

        public bool Active { get; set; }
        public string RootServiceName { get; set; }
        public MethodValidation MethodValidation { get; set; }
        public IEnumerable<IModuleEndpoint> Endpoints { get; set; }

        public void AddEndpoint(EndpointConfiguration endpoint)
        {
            _endpoints.Add(endpoint);
        }
    }
}
