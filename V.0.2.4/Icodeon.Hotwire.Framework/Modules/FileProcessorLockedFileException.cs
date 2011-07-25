using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Modules
{
    [DataContract]
    public class FileProcessorLockedFileException : HotwireExceptionDTO
    {
        public FileProcessorLockedFileException(Exception ex) : base(ex)
        {
            Message = string.Format("File was locked and could not be moved to error folder! : {0}", Message);
        }
    }
}
