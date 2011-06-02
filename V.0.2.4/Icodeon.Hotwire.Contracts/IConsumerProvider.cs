namespace Icodeon.Hotwire.Contracts
{
    public interface IConsumerProvider 
    {
        string GetConsumerSecret(string consumerKey);
    }
}
