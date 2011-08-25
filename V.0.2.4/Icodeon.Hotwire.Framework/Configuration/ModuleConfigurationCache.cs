using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.DTOs;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class ModuleConfigurationCache
    {
        private string _sectionName;
        private string _key;
        private readonly string _configurationSectionName;
        private readonly IAppCache _appCache;

        public ModuleConfigurationCache(string configurationSectionName, IAppCache appCache)
        {
            _configurationSectionName = configurationSectionName;
            _appCache = appCache;
            _key = string.Format("hotwire.config.{0}", _configurationSectionName);
            _sectionName = string.Format(@"{0}/{1}", ConfigurationBase.SectionGroup, _configurationSectionName);
        }


        public IModuleConfiguration Configuration
        {
            get
            {
                var config = (IModuleConfiguration)_appCache.Get(_key);
                if (config==null) return RefreshConfigurationFromWebOrAppConfig();
                return config;
            }
            set
            {
                _appCache.Set(_key,value);
            }
        }

        
        /// <summary>
        /// read the Configuration property you just want to read the configuration but not affect 
        /// the cache access the Configuration property. Refresh will "refresh" the app cache from a fresh read of the configuration files.
        /// </summary>
        /// <returns></returns>
        public IModuleConfiguration RefreshConfigurationFromWebOrAppConfig()
        {

            IModuleConfiguration config = (IModuleConfiguration)ConfigurationManager.GetSection(_sectionName);
            // if we use different configuration types, then this class needs to update to check the type being returned (switch) and assign to appropriate DTOs
            var dto = new ModuleConfigurationDTO
                          {
                              Active = config.Active,
                              MethodValidation = config.MethodValidation,
                              RootServiceName = config.RootServiceName,
                              Endpoints = config.Endpoints.Select( e=> (IModuleEndpoint)e.ToDTO()).ToList(),
                              Debug = config.Debug
                          };
            _appCache.Set(_key,dto);
            return dto;
        }

    }
}
