using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Web;
using Icodeon.Hotwire.Framework.DTOs;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Contracts
{
    [Serializable]
    [DataContract]
    public class EnqueueRequestDTO : IEnqueueRequest
    {

        public EnqueueRequestDTO()
        {
            ExtraParameters = new List<NameValueDTO>();
        }

        // todo move to request validator
        public static void validateRequiredParameters(NameValueCollection parameters)
        {
            var keys = parameters.AllKeys;
            var missingParams = RequiredFields.Where(rf => !keys.Contains(rf));
            if (missingParams.Count()>0)
            {
                throw new ArgumentException("The following parameters are required and were not provided:" + String.Join(",", missingParams));
            }
        }


        public static IEnumerable<string> RequiredFields
        {
            get
            {
                yield return ParamHotwireVersion;
                yield return ParamUserId;
                yield return ParamExtResourcelinkcontent;
                yield return ParamResourceId;
                yield return ParamExtResourceLinkAuthoriseType;
            }
        }

        public const string ParamHotwireVersion = "hotwire_version";
        public const string ParamUserId = "user_id";
        public const string ParamExtResourcelinkcontent = "ext_resource_link_content";
        public const string ParamExtResourceLinkAuthoriseType = "ext_resource_link_authorise_type";
        public const string ParamResourceId = "resource_id";
        public const string ParamResourceTitle = "resource_title";
        public const string ParamQueueCategory = "queue_category";
        public const string ParamQueuePriority = "queue_priority";

        

        public EnqueueRequestDTO(NameValueCollection parameters) : this()
        {
            HotwireVersion = parameters[ParamHotwireVersion];
            UserId = parameters[ParamUserId];
            ExtResourceLinkContent = parameters[ParamExtResourcelinkcontent];
            ExtResourceLinkAuthoriseType = parameters[ParamExtResourceLinkAuthoriseType];
            ResourceFile = Path.GetFileName(ExtResourceLinkContent);
            ResourceId = parameters[ParamResourceId];
            ResourceTitle = parameters[ParamResourceTitle];
            QueueCategory = parameters[ParamQueueCategory];
            
            
            QueuePriority = Int32.Parse(parameters[ParamQueuePriority] ?? "0");

            var restOfParameters = parameters.AllKeys.Except( new[] {
                        ParamExtResourceLinkAuthoriseType,
                        ParamHotwireVersion, 
                        ParamUserId, 
                        ParamExtResourcelinkcontent, 
                        ParamResourceId, 
                        ParamResourceTitle, 
                        ParamQueueCategory, 
                        ParamQueuePriority });

            foreach (var key in restOfParameters)
            {
                ExtraParameters.Add(new NameValueDTO() { Name =key, Value = parameters[key]});
            }
        }

        [DataMember]
        public List<NameValueDTO> ExtraParameters { get; set; }

        [DataMember]
        public QueueStatus QueueStatus { get; set; }

        [DataMember]
        public string TransactionId { get; set; }

        [DataMember]
        public string HotwireVersion { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string ExtResourceLinkAuthoriseType { get; set; }

        [DataMember]
        public string ExtResourceLinkContent { get; set; }

        [DataMember]
        public string ResourceFile { get; set; }

        [DataMember]
        public string ResourceId { get; set; }

        [DataMember]
        public string ResourceTitle { get; set; }

        [DataMember]
        public string QueueName { get; set; }

        [DataMember]
        public int QueuePriority { get; set; }

        [DataMember]
        public string QueueCategory { get; set; }
     
        public string GetTrackingNumber()
        {
            return FormatTrackingNumber(TransactionId, ResourceFile);
        }

        public static string FormatTrackingNumber(string transactionId, string resourceFile)
        {
            return String.Format("{0}_{1}", transactionId, resourceFile);
        }

        public string GetImportFileName()
        {
            return String.Format("{0}_{1}.import", TransactionId, ResourceFile);
        }

        public NameValueCollection ToUnderScoreIcodeonCCPNamedNameValueCollectionPlusExtraHotwireParamsAndAnyExtraParamsPostedByClient()
        {
            NameValueCollection coll = new NameValueCollection()
                                           {
                                               {"TransactionId", TransactionId},
                                               {"TrackingNumber",GetTrackingNumber() },
                                               {"QueueStatus", QueueStatus.ToString()},
                                               {"ResourceFile", ResourceFile},
                                               {"QueueName", QueueName},
                                               {"QueuePriority", QueuePriority.ToString()},
                                               {"QueueCategory", QueueCategory },
                                               {ParamHotwireVersion,HotwireVersion },
                                               {ParamUserId, UserId },
                                               {ParamExtResourceLinkAuthoriseType, ExtResourceLinkAuthoriseType},
                                               {ParamExtResourcelinkcontent, ExtResourceLinkContent},
                                               {ParamResourceId, ResourceId },
                                               {ParamResourceTitle, ResourceTitle},
                                               {ParamQueueCategory, QueueCategory },
                                               {ParamQueuePriority, QueuePriority.ToString()}
                                           };
            ExtraParameters.ForEach(ep => coll.Add(ep.Name, ep.Value));
            return coll;
        }

        public NameValueCollection ToDebugListOfParameters()
        {
            // todo use reflection to get all the property values!
            NameValueCollection coll = new NameValueCollection()
                                           {
                                               {"TransactionId", TransactionId},
                                               {"TrackingNumber",GetTrackingNumber() },
                                               {"QueueStatus", QueueStatus.ToString()},
                                               {"ResourceFile", ResourceFile},
                                               {"QueueName", QueueName},
                                               {"QueuePriority", QueuePriority.ToString()},
                                               {"QueueCategory", QueueCategory },
                                               {"HotwireVersion",HotwireVersion },
                                               {"UserId", UserId },
                                               {"ExtResourceLinkContent", ExtResourceLinkContent},
                                               {"ExtResourceLinkAuthoriseType", ExtResourceLinkAuthoriseType },
                                               {"ResourceId", ResourceId },
                                               {"ResourceTitle", ResourceTitle},
                                               {"QueueCategory", QueueCategory },
                                               {"QueuePriority", QueuePriority.ToString()}
                                           };
            ExtraParameters.ForEach(ep => coll.Add(ep.Name, ep.Value));
            return coll;
        }

        public static string GetTrackingNumberFromImportFileName(string importFileNameOrFilePath)
        {
            string filename = Path.GetFileName(importFileNameOrFilePath);
            return filename.Substring(0, filename.Length - ImportExtension.Length);
        }


        public const string ImportExtension = ".import";
        public const string ErrorExtension = ".error";
        public const string SkippedExtension = ".skipped";

        public static string AddSkippedExtension(string trackingOrImportNumber)
        {
            return String.Format("{0}{1}", trackingOrImportNumber, SkippedExtension);
        }
        
        public static string AddErrorExtension(string trackingNumber)
        {
            return String.Format("{0}{1}", trackingNumber, ErrorExtension);
        }

        public static string AddImportExtension(string trackingNumber)
        {
            return String.Format("{0}{1}", trackingNumber,ImportExtension);
        }
    }
}