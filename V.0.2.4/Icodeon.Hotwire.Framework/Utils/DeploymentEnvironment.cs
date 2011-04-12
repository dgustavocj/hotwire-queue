using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Utils
{
    public static class DeploymentEnvironment
    {
        public static bool IsRELEASE
        {
            get
            {

#if DEBUG
                return false;
#else
            return true;
#endif
            }

        }

        public static bool IsDEBUG
        {
            get
            {
#if DEBUG
                return true;
#else
            return false;
#endif

            }
        }


    }
}
