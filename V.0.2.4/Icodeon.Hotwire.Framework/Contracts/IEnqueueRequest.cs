using System.Collections.Generic;
using System.Runtime.Serialization;
using Icodeon.Hotwire.Framework.DTOs;

namespace Icodeon.Hotwire.Framework.Contracts
{
    public interface IEnqueueRequest
    {
        List<NameValueDTO> ExtraParameters { get; set; }

        [DataMember]
        string TransactionId { get; set; }

        [DataMember]
        string HotwireVersion { get; set; }

        [DataMember]
        string UserId { get; set; }

        [DataMember]
        string ExtResourceLinkContent { get; set; }

        [DataMember]
        string ResourceId { get; set; }

        [DataMember]
        string ResourceFile { get; set; }

        [DataMember]
        string ResourceTitle { get; set; }

        [DataMember]
        string QueueName { get; set; }

        [DataMember]
        int QueuePriority { get; set; }

        [DataMember]
        string QueueCategory { get; set; }

    }
}