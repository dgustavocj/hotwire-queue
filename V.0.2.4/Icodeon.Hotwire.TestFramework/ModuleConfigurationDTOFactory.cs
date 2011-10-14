using System.Collections.Generic;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.DTOs;
using Icodeon.Hotwire.Framework.MediaTypes;
using Icodeon.Hotwire.TestFramework.Mocks;

namespace Icodeon.Hotwire.TestFramework
{
    public static class ModuleConfigurationDTOFactory
    {
        public static ModuleConfigurationDTO GivenModuleConfigurationForMockModule()
        {
            var configuration = new ModuleConfigurationDTO
            {
                Active = true,
                Debug = false,
                ExclusiveUse = true,
                MethodValidation = MethodValidation.beforeUriValidation,
                Endpoints = new List<IModuleEndpoint> { new EndpointDTO() { 
                    Action = MockModule.ActionHttpModuleException, 
                    Active = true, 
                    HttpMethods = new[] {"GET"},
                    MediaType = eMediaType.text,
                    Name = "endpoint1", 
                    UriTemplateString = "/throw/httpexception" }}
            };
            return configuration;
        }


        public static ModuleConfigurationDTO GivenModuleConfiguration(string rootServiceName)
        {
            var configuration = new ModuleConfigurationDTO
            {
                RootServiceName = rootServiceName,
                Active = true,
                Debug = false,
                MethodValidation = MethodValidation.beforeUriValidation,
                Endpoints = new List<IModuleEndpoint>()
            };
            return configuration;
        }

    }
}