using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Icodeon.Hotwire.Framework.Configuration;

namespace Icodeon.Hotwire.Framework.Contracts
{
    [Serializable]
    [DataContract(Namespace = Constants.Namespaces.ICODEON_HOTWIRE_BETA_V0_2)]
    public class QueuedResource 
    {

        public QueuedResource() { }

        public QueuedResource(string trackingNumber)
        {
            TrackingNumber = trackingNumber;
        }


        [DataMember]
        public string TrackingNumber { get; set; }

    }
}