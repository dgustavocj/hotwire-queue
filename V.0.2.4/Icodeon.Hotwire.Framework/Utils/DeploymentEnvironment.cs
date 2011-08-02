using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Utils
{
    public static class DeploymentEnvironment
    {

        public enum BuildConfiguration
        {
            DEBUG, RELEASE

        }

        public static BuildConfiguration CurrentBuildConfiguration
        {
            get
            {
#if DEBUG
                return BuildConfiguration.DEBUG;
#endif
#if RELEASE
                return BuildConfiguration.RELEASE;
#endif
            }
        }
    }
}
