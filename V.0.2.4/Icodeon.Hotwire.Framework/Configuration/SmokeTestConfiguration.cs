using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Modules;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class SmokeTestConfiguration : ModuleConfigurationBase
    {
        public override string GetConfigurationSectionName()
        {
            return SmokeTestModule.SmokeTestSectionName;
        }
    }
}
