using System.Collections.Generic;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class MockAppCache : IAppCache
    {
        public List<string> SetHistory { get; private set; }

        public MockAppCache()
        {
            _cache = new Dictionary<string, object>();
            SetHistory = new List<string>();
        }

        private Dictionary<string, object> _cache;

        public void Set(string key, object source)
        {
            SetHistory.Add(key);
            _cache[key] = source;
        }

        public object Get(string key)
        {
            if (!_cache.ContainsKey(key)) return null;
            return _cache[key];
        }

        public void Lock()
        {
            // do nothing
        }

        public void Unlock()
        {
            // do nothing
        }
    }
}