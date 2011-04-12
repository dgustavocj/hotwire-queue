using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Modules
{
    //NOTE: ADH: 09.04.2011 looks like this class should  be deleted and all usages of
    // procesfileResponseDTO should be replaced with EnqueueRequestDTO
    // or else something needs to be fixed? (need to check it out)

    [DataContract]
    public class ProcessFileResponseDTO : EnqueueRequestDTO
    {
        public ProcessFileResponseDTO() : base()
        {
        }
    }
}
