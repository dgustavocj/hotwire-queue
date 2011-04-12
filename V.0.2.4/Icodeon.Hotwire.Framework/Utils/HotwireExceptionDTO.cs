using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Icodeon.Hotwire.Framework.Utils
{

    // See WebException possibly replace with that 

    [DataContract]
    public class HotwireExceptionDTO
    {
        public HotwireExceptionDTO(Exception ex)
        {
            Message = ex.Message ?? "null";
            StackTrace = ex.StackTrace ?? "null";
            Source = ex.Source ?? "null";
            Exception = ex.ToString();
            InnerException = ex.InnerException == null ? "null" : ex.InnerException.ToString();
        }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string StackTrace { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public string Exception { get; set; }

        [DataMember]
        public string InnerException { get; set; }

    }
}
