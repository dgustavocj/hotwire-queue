using System.Runtime.Serialization;
using Icodeon.Hotwire.Framework.Configuration;

namespace Icodeon.Hotwire.Framework.DTOs
{
    [DataContract(Namespace = Constants.Namespaces.ICODEON_HOTWIRE_BETA_V0_2)]
    public class NameValueDTO
    {
        // must have public empty constructor otherwise Xml Serializer cannot serialize this type.
        // compiler used to create this, looks like that is no longer the case, huh! Leave this out and you get serialisation errors with DataContractSerializer
        public NameValueDTO()
        {

        }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}