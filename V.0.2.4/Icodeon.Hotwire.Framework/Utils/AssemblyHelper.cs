using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Icodeon.Hotwire.Framework.Utils
{
    public static class AssemblyHelper
    {
        public static string FrameworkVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public static string ExecutingAssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }


        public static string ServiceVersion
        {
            get
            {
                return Assembly.GetCallingAssembly().GetName().Version.ToString();
            }

        }




    }
}
