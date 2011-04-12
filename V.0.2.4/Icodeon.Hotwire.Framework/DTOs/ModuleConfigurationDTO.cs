using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Configuration;

namespace Icodeon.Hotwire.Framework.DTOs
{
    public class ModuleConfigurationDTO : IModuleConfiguration
    {
        public ModuleConfigurationDTO()
        {
            Endpoints = new List<IModuleEndpoint>();
        }

        public bool Active { get; set; }

        public string RootServiceName { get; set; }

        public MethodValidation MethodValidation { get; set; }

        public List<IModuleEndpoint> Endpoints { get; set; }
    }
}
