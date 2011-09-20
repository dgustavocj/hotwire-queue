using System;

namespace Icodeon.Hotwire.Framework.DAL.Params
{
    public class CacheRequestParams
    {
        private Guid _salt;
        private string _url;
        private int _msToExpire;

        public CacheRequestParams(Guid salt, string url, int msToExpire)
        {
            _salt = salt;
            _url = url;
            _msToExpire = msToExpire;
        }

        public Guid Salt
        {
            get { return _salt; }
        }

        public string Url
        {
            get { return _url; }
        }

        public int MsToExpire
        {
            get { return _msToExpire; }
        }
    }
}