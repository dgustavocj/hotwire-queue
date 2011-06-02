using Icodeon.Hotwire.Framework.Configuration;

namespace Icodeon.Hotwire.Tests.Internal
{
    public class TestModuleConfiguration : ModuleConfigurationBase
    {
        protected override string GetConfigurationSectionName()
        {
            return "test-module-config";
        }
    }
}