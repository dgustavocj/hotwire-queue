using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Contracts
{
    public interface IAppCounter
    {
        int ReadCounter();
        void DecCounter();
        void IncCounter();
    }
}
