namespace Icodeon.Hotwire.Framework.Configuration
{
    public interface ISSLEMailErrorHandlerConfig : IConfigurationSection
    {
        // TODO: encrypt values provided in the section below: 
        string FromAddress { get; set; }
        string[] ToAddresses { get; set; }
        string SubjectLinePrefix { get; set; }
        int TimeoutSeconds { get; set; }
    }
}