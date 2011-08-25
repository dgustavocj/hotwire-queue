using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.DTOs
{
    [DataContract]
    public class EndpointDTO : IModuleEndpoint
    {

        public UriTemplate UriTemplate { get; set; }

        [DataMember]
        public string UriTemplateString
        {
            get { return UriTemplate.ToString(); }
            set { UriTemplate = new UriTemplate(value);}
        }

        [DataMember]
        public IEnumerable<string> HttpMethods { get; set; }

        [DataMember]
        public MediaTypes.eMediaType MediaType { get; set; }

        [DataMember]
        public string Action { get; set; }

        [DataMember]
        public bool Active { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public SecurityType Security { get; set; }

        [DataMember]
        public string PrivateKey { get; set; }

    }
}
