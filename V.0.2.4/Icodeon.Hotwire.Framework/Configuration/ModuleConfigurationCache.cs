using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using Icodeon.Hotwire.Framework.DTOs;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class ModuleConfigurationCache
    {
        private string _sectionName;
        private string _key;
        private readonly string _configurationSectionName;
        private readonly HttpApplicationState _app;

        public ModuleConfigurationCache(string configurationSectionName, HttpApplicationState app)
        {
            _configurationSectionName = configurationSectionName;
            _app = app;
            _key = string.Format("hotwire.config.{0}", _configurationSectionName);
            _sectionName = string.Format(@"{0}/{1}", Constants.Configuration.SectionGroup, _configurationSectionName);
        }


        public IModuleConfiguration Configuration
        {
            get
            {
                var config = (IModuleConfiguration)_app[_key];
                if (config==null) return RefreshConfiguration();
                return config;
            }
            set
            {
                _app[_key] = value;
            }
        }

        
        /// <summary>
        /// read the Configuration property you just want to read the configuration but not affect 
        /// the cache access the Configuration property. Refresh will "refresh" the app cache from a fresh read of the configuration files.
        /// </summary>
        /// <returns></returns>
        public IModuleConfiguration RefreshConfiguration()
        {

            IModuleConfiguration config = (IModuleConfiguration)ConfigurationManager.GetSection(_sectionName);
            // if we use different configuration types, then this class needs to update to check the type being returned (switch) and assign to appropriate DTOs
            var dto = new ModuleConfigurationDTO
                          {
                              Active = config.Active,
                              MethodValidation = config.MethodValidation,
                              RootServiceName = config.RootServiceName,
                              Endpoints = config.Endpoints.Select( e=> new EndpointDTO()
                                {
                                    Security = e.Security,
                                    Action = e.Action,
                                    Active = e.Active,
                                    HttpMethods = e.HttpMethods,
                                    MediaType =  e.MediaType,
                                    Name = e.Name,
                                    UriTemplate = e.UriTemplate
                                }).ToList<IModuleEndpoint>()
                          };
            _app[_key] = dto;
            return dto;
        }

        public static IModuleConfiguration ReadQueueModuleConfiguration(HttpApplicationState applicationState)
        {
            return new ModuleConfigurationCache(Constants.Configuration.QueuesSectionName,applicationState).Configuration; 
        }

        public static IModuleConfiguration ReadSmokeTestConfiguration(HttpApplicationState applicationState)
        {
            return new ModuleConfigurationCache(Constants.Configuration.SmokeTestSectionName, applicationState).Configuration;
        }
    }
}
