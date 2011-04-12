using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Contracts
{
    public static class Parameters
    {
        // the headers might change from version to version
        public static class Ver0_1
        {
            public static class Headers
            {
                public const string hotwire_version_framework = "hotwire_version_framework";
                public const string hotwire_version_service = "hotwire_version_service";
            }

        }

    }
}