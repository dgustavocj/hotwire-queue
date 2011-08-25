using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public abstract class ModuleConfigurationBase : ConfigurationBase, IModuleConfiguration
    {
        [ConfigurationProperty("debug", IsRequired = false)]
        public bool Debug
        {
            get { return (bool)this["debug"]; }
            set { this["debug"] = value; }
        }

        [ConfigurationProperty("rootServiceName", IsRequired = true)]
        public string RootServiceName
        {
            get { return (string)this["rootServiceName"]; }
            set { this["rootServiceName"] = value; }
        }

        [ConfigurationProperty("methodValidation", IsRequired = false,DefaultValue = MethodValidation.beforeUriValidation)]
        public MethodValidation MethodValidation
        {
            get { return (MethodValidation)this["methodValidation"]; }
            set { this["methodValidation"] = value; }
        }

        [ConfigurationProperty("active", IsRequired = true)]
        public bool Active
        {
            get { return (bool)this["active"]; }
            set { this["active"] = value; }
        }

        // this attributes defines the xml collection element name
        [ConfigurationProperty("endpoints")]
        public EndpointCollection EndpointCollection
        {
            get { return (EndpointCollection)this["endpoints"]; }
        }

        public List<IModuleEndpoint> Endpoints
        {
            get { return EndpointCollection.Cast<IModuleEndpoint>().ToList(); }
        }

        public void AddRange(IEnumerable<EndpointConfiguration> endpoints)
        {
            foreach (var moduleEndpoint in endpoints)
            {
                AddEndpoint(moduleEndpoint);
            }
        }

        public void AddEndpoint(EndpointConfiguration endpoint)
        {
            EndpointCollection.Add(endpoint);
        }

    } // class
} // namespace 