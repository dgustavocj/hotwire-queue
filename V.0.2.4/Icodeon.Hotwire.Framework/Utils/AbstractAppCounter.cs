using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Utils
{
    public abstract class AbstractAppCounter : IAppCounter
    {
        private HttpApplicationState _applicationState;
        private string _key;

        protected AbstractAppCounter(HttpApplicationState applicationState, string counterName)
        {
            _applicationState = applicationState;
            _key= counterName;

        }

        public int ReadCounter()
        {
            return (int)(_applicationState[_key] ?? 0);
        }

        public void DecCounter()
        {
            _applicationState.Lock();
            _applicationState[_key] = ReadCounter() - 1;
            _applicationState.UnLock();
        }

        public void IncCounter()
        {
            _applicationState.Lock();
            _applicationState[_key] = ReadCounter() + 1;
            _applicationState.UnLock();
        }
    }
}
