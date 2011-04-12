using System;

namespace Icodeon.Hotwire.Framework.Contracts
{
    public class ServiceRequestEventArgs : EventArgs
    {
        public ServiceRequestEventArgs(string uri, string method)
        {
            Uri = uri;
            Method = method;
        }

        public string Uri { get; set; }
        public string Method { get; set; }
    }
}