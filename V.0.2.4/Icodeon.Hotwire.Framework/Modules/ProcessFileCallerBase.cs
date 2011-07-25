using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Modules
{
    public abstract class ProcessFileCallerBase
    {

        public abstract void CallProcessFileWaitForComplete(string trackingNumber);
    }
}
