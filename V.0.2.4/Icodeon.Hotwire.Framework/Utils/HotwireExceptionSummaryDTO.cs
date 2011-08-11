using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Icodeon.Hotwire.Framework.Utils
{
    [DataContract]
    public class HotwireExceptionSummaryDTO
    {
        public HotwireExceptionSummaryDTO(Exception ex)
        {
            Message = ex.Message ?? "null";
            ExceptionTypeName = ex.GetType().ToString();
        }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string ExceptionTypeName { get; set; }
    }
}
