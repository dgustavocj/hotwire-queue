using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public class ProcessFileConfiguration : ModuleConfigurationBase
    {
        public const string ProcessFileSectionName = "processFile";

        public override string GetConfigurationSectionName()
        {
            return ProcessFileSectionName;
        }
    }
}
