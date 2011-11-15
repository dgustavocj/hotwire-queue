using System;

namespace Icodeon.Hotwire.Framework.FolderWatcher
{
    public class CommandArgument
    {
        public CommandArgument()
        {
            Cmd = "";
            Argument = "";
        }

        public string Cmd { get; set; }
        public string Argument { get; set; }
        public void AssertArgumentRequired()
        {
            if (string.IsNullOrWhiteSpace(Argument)) throw new ArgumentNullException("'" + Cmd + "' requires an argument, which is not provided. ");
        }
    }
}