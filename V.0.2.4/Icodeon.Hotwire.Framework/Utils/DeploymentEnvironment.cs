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
            DEBUG, RELEASE, LIVETEST

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
#if TESTSERVER
                return BuildConfiguration.LIVETEST;
#endif
            }
        }

        public static bool IsTestServer
        {
            get
            {

#if TESTSERVER
                return false;
#else
            return true;
#endif
            }

        }


        public static bool IsDebug
        {
            get
            {
#if (DEBUG)
                return true;
#else
            return false;
#endif

            }
        }

        public static bool IsDebugOrRelease
        {
            get
            {
#if (DEBUG || RELEASE)
                return true;
#else
            return false;
#endif

            }
        }


    }
}
