using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Icodeon.Hotwire.Framework.Contracts
{
    [DataContract]
    public class CartridgeDTO
    {
        [DataMember]
        public string TransactionId { get; set; }
        [DataMember]
        public string ResourceId { get; set; }
        [DataMember]
        public string ResourceTitle { get; set; }
        [DataMember]
        public string ResourceFile { get; set; }
        [DataMember]
        public SecurityType ResourceAuthoriseType { get; set; }
        [DataMember]
        public string ResourceUri { get; set; }

        //TODO replace with XML serialiser
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5}", TransactionId, ResourceId,ResourceTitle, ResourceFile, ResourceAuthoriseType,ResourceUri);
        }


        public static List<CartridgeDTO> ParseLines(IEnumerable<string> lines)
        {
            var files = lines.Select(line =>
                                         {
                                             var columns = line.Split(new[] {','}, StringSplitOptions.None);
                                             var retval = new CartridgeDTO
                                                              {
                                                                  TransactionId = columns[0],
                                                                  ResourceId = columns[1],
                                                                  ResourceTitle = columns[2],
                                                                  ResourceFile = columns[3],
                                                                  ResourceAuthoriseType = (SecurityType)Enum.Parse(typeof(SecurityType),columns[4]),
                                                                  ResourceUri = columns[5]
                                                              };
                                             return retval;
                                         });
            return files.ToList();
        }


        public static CartridgeDTO ParseLine(string line)
        {
            string filename = line
                .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries)
                .First(l => l.Contains(".zip"))
                .TrimEnd(new[] { '<' });

            string resourceId = filename.Substring(0, filename.LastIndexOf('_'));
            string uri = line.Substring(0, line.LastIndexOf('/') - 1);

            return new CartridgeDTO {
                                          ResourceFile = filename,
                                          ResourceId = resourceId,
                                          ResourceTitle = null,
                                          TransactionId = Guid.NewGuid().ToString(),
                                          ResourceAuthoriseType =  SecurityType.none,
                                          ResourceUri = uri
                                      };
        }
    }
}