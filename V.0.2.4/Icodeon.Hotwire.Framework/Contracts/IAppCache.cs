using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Contracts
{
    public interface IAppCache
    {
        void Set(string key, object source);
        object Get(string key);
        void Lock();
        void Unlock();
    }
}
