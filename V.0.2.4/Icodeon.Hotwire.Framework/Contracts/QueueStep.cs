using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Icodeon.Hotwire.Framework.Configuration;

namespace Icodeon.Hotwire.Framework.Contracts
{
    [DataContract(Namespace = Constants.Namespaces.ICODEON_HOTWIRE_BETA_V0_2)]
    public class QueueStep
    {
        [DataMember]
        public DateTime? Started { get; set; }
        [DataMember]
        public DateTime? Finished { get; set; }
        [DataMember]
        public QueueStatus Status { get; set; }
    }
}