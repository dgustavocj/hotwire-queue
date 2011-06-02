using System.Collections.Specialized;

namespace Icodeon.Hotwire.Contracts
{
    public interface IFileProcessorProvider 
    {
        //TODO: Change resource_file to "trackingNumber"
        void ProcessFile(string resource_file, string transaction_id, NameValueCollection requestParams);
    }
}