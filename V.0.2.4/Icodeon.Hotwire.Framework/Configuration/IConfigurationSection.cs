namespace Icodeon.Hotwire.Framework.Configuration
{
    public interface IConfigurationSection
    {
        string GetConfigurationSectionName();
        string ConfigurationSectionName { get; set; }
    }
}