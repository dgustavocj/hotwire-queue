using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Security
{
    public interface INonceCacheProvider
    {
        // nonce cache provider "seconds" resolution is double, so that we can write much faster unit tests that don't have to wait a whole second
        void CacheNonce(string userId, Guid salt, string Mac, double seconds);
        bool Exist(string userId, Guid salt, string Mac);
        void Expire(string userId, Guid salt, string Mac);
    }
}
