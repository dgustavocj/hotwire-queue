using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Icodeon.Hotwire.Framework.Contracts
{
    [DataContract]
    public enum QueueStatus
    {
        /// <summary>
        /// Safe to download : has not been downloaded and/or is not currently being downloaded or any other action
        /// </summary>
        [EnumMember]
        None,

        [EnumMember]
        QueuedForDownloading,

        [EnumMember]
        QueuedForProcessing,

        [EnumMember]
        Downloading,

        [EnumMember]
        Processing,
        
        [EnumMember]
        Processed,
        
        [EnumMember]
        ProcessError,

        [EnumMember]
        DownloadError
    }

    public static class QueueStatusExtensions
    {
        public static string ToDisplayString(this List<QueueStatus> src)
        {
            StringBuilder sb = new StringBuilder();
            src.ForEach( s => sb.Append(s + ",") );
            return sb.ToString().TrimEnd(new[] {','});
        }
    }

}