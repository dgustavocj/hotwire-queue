using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Contracts
{
    public class ProcessFileRequestResult
    {
        // returns the total number of threads busy processing files
        public int TotalThreads { get; set; }
        public int CntFilesWaitingToBeProcessed { get; set; }
    }
}
