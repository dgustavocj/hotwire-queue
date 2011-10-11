using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NLog;

// **  NB! don't wait for exit if redirecting output see :                     
// **  http://msdn.microsoft.com/en-us/library/system.diagnostics.processstartinfo.redirectstandardoutput.aspx

namespace Icodeon.Hotwire.Framework.Deployment
{
    public class CommandShell
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly string _currentDirectory;
        public string StartFolder { get; set; }

        public CommandShell(string currentDirectory)
        {
            _currentDirectory = currentDirectory;
            _logger.Debug("new CommandShell created.");
            _logger.Trace("Run from directory set to [{0}].", currentDirectory);
        }

        public string ShellWithOutputToConsole(string command)
        {
            return Shell(command, false);
        }

        public string ShellWithOutputRedirectAsString(string command)
        {
            return Shell(command, true);
        }

        private string Shell(string cmd, bool redirectOutput)
        {
            _logger.Debug("Shell(cmd:='{0}',redirectOutput:={1})",cmd,redirectOutput);
            var arguments = string.Format("/c {0}", cmd);
            var info = new ProcessStartInfo("cmd", arguments)
                           {
                               CreateNoWindow = true,
                               RedirectStandardOutput = true,
                               UseShellExecute = false,
                               WorkingDirectory = StartFolder
                           };
            using (var process = new Process() {StartInfo = info})
            {
                if (redirectOutput)
                {
                    process.StartInfo.RedirectStandardOutput = true;
                    process.Start();
                    var output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return output;
                }
                else
                {
                    process.Start();
                    process.WaitForExit();
                    return "";
                }
            }
        }



    }
}
