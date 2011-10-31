using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Modules
{
    [DataContract]
    public class QueueStatusDTO
    {
        [DataMember]
        public QueueStatus Status { get; set; }

        [DataMember]
        public string TrackingNumber { get; set; }

        [DataMember]
        public string DebugInfo { get; set; }

    }

}
