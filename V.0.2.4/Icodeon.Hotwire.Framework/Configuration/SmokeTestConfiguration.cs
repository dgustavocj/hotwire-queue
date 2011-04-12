using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class SmokeTestConfiguration : ModuleConfigurationBase
    {
        protected override string GetConfigurationSectionName()
        {
            return Constants.Configuration.SmokeTestSectionName;
        }
    }
}
