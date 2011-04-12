using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icodeon.Hotwire.Framework.Contracts
{
    public static class Constants
    {

        //TODO: create a single hotwire configuration file that is shared between projects and stored in the solution root.

        /// <summary>
        /// this is the key in the app.config file that will contain the name of the Solution Folder marker file.
        /// </summary>
        public const string SolutionFolderMarkerFile = "HotwireSolutionFolderMarkerFile.txt";

        public static class ProjectFolders
        {
            public const string Icodeon_Hotwire_TestAspNet = "Icodeon.Hotwire.TestAspNet";
        }

        public static class ProjectPorts
        {
            public const int TestAspNet = 54144;
        }

        public static class OAuth
        {
            public static class RequestParameterNames
            {
                public const string USER_ID = "user_id";
            }
        }
        public static class Namespaces
        {
            public const string ICODEON_HOTWIRE_BETA_V0_2 = "http://www.icodeon.com/hotwire/beta/v0.2";    
        }
        
    }
}