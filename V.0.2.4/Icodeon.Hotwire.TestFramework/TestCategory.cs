using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.TestFramework
{
    public static class TestCategory
    {
        // integration slow tests using IIS or cassini websites
        public const string Integration = "Integration";

        // requires a live internet connection to be able to run
        public const string Online = "Online";

        // tests that have given us some intermittent failures in the past...
        // we want to group these together so that we can run them seperately and should not hold up the build.
        // alternatively we WANT to know about them, i.e. can group by "Warning" tests, so that we can see which have given us 
        // some problems before and we may have to deal with later.
        public const string Warning = "Warning";

        // anything NOT marked as ONLINE or SLOW is by definition "OFFLINE" and FAST!
    }
}
