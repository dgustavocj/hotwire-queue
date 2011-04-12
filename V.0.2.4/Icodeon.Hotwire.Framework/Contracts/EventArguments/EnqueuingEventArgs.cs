using System.Collections.Specialized;

namespace Icodeon.Hotwire.Framework.Contracts
{
    public class EnqueuingEventArgs : ServiceRequestEventArgs
    {
        public string Endpoint { get; set; }
        public NameValueCollection BodyParameters { get; private set; } 

        public EnqueuingEventArgs(string uri, string method, string endpoint, NameValueCollection bodyParameters) : base(uri, method)
        {
            Endpoint = endpoint;
            BodyParameters = bodyParameters;
        }
    }
}