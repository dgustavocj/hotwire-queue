using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;


namespace Icodeon.Hotwire.TestFramework
{
    public static class Approval
    {
        public static void CheckIfApprovalExists(string className, string testName)
        {
            string commaSeperatedDirectories= ConfigurationManager.AppSettings["approval-test-directories"];
            if (commaSeperatedDirectories == null) throw new ApplicationException("Could not find approval-test-directories in appsettings.");
            var directories = commaSeperatedDirectories.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();
            
            if (!directories.Any(dir => File.Exists(ApprovalFile(dir,className, testName))))
            {
                string approvalFile = ApprovalFile(directories[0],className, testName);
                throw new ApplicationException("No approval found for " + testName + "\nIf you want to approve this test, please create a placeholder approval file in '" + approvalFile + "'. This approval is local to each developer, and should not be checked in to source control and is suitable as a temporary placeholder while waiting for replacement tests to be provided by outsource coder or tester. Approvals will work locally, but 'BY DESIGN' MUST NOT pass on the build server, so that you remember to replace with more reliable automated tests.");
            }
                
        }

        private static string ApprovalFile(string dir, string className, string testName)
        {
            return Path.Combine(dir, className, testName + ".approval");
        }

    }


}
