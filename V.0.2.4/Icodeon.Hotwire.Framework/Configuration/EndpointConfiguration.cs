using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.MediaTypes;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class EndpointConfiguration : ConfigurationElement, IModuleEndpoint
    {

        [ConfigurationProperty("uriTemplate", IsKey = true, IsRequired = true)]
        public string UriTemplateString
        {
            get { return (string)this["uriTemplate"]; }
            set { this["uriTemplate"] = value; }
        }

        public UriTemplate UriTemplate
        {
            get { return new UriTemplate(UriTemplateString); }
            set { UriTemplateString = value.ToString(); }
        }

        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        public IEnumerable<string> HttpMethods
        {
            get { return CommaDelimitedListHttpMethods.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries); }
            set { CommaDelimitedListHttpMethods = string.Join(",", value); }
        }

        //TODO: add in type validation (PUT,GET,DELETE,POST etc.)
        [ConfigurationProperty("httpMethods", IsKey = false, IsRequired = true)]
        public string CommaDelimitedListHttpMethods
        {
            get { return (string)this["httpMethods"]; }
            set { this["httpMethods"] = value; }
        }

        [ConfigurationProperty("mediaType", IsKey = false, IsRequired = true)]
        public eMediaType MediaType
        {
            get { return (eMediaType)this["mediaType"]; }
            set { this["mediaType"] = value; }
        }

        [ConfigurationProperty("active", IsKey = false, IsRequired = true)]
        public bool Active
        {
            get { return (bool)this["active"]; }
            set { this["active"] = value; }
        }

        [ConfigurationProperty("action", IsKey = false, IsRequired = true)]
        public string Action
        {
            get { return (string)this["action"]; }
            set { this["action"] = value; }
        }

        public override string ToString()
        {
            return string.Format("Action:{0},Active:{1},MediaType:{2},HttpMethods:{3},Name:{4},UriTemplate:{5}",
                                 this.Action, Active, this.MediaType, HttpMethods,Name, UriTemplate);
        }

        [ConfigurationProperty("security", IsKey = false, IsRequired = false,DefaultValue = "none")]
        public SecurityType Security
        {
            get { return (SecurityType)this["security"]; }
            set { this["security"] = value; }
        }
    }
}
