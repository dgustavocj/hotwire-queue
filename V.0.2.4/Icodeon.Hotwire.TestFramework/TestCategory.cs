using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.TestFramework
{
    public static class TestCategory
    {
        // is a slow test
        public const string Slow = "Slow";
        // requires a live internet connection to be able to run
        public const string Online = "Online";


        // anything NOT marked as ONLINE or SLOW is by definition "OFFLINE" and FAST!
    }
}
