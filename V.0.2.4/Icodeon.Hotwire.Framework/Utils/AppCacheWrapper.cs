using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Utils
{
    public class AppCacheWrapper : IAppCache
    {
        private readonly HttpApplicationState _appstate;

        public AppCacheWrapper(HttpApplicationState appstate) {
            _appstate = appstate;
        }

        public IAppCache Create()
        {
            return new AppCacheWrapper(HttpContext.Current.Application);
        }

        public void Lock()
        {
            _appstate.Lock();
        }

        public void Unlock()
        {
            _appstate.UnLock();
        }

        public void Set(string name, object source)
        {
            _appstate[name] = source;
        }

        public object Get(string key) 
        {
            return _appstate[key];
        }
    }


}
