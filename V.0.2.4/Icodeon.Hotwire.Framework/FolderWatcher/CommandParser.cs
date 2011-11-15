using System.Linq;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.FolderWatcher
{
    public static class CommandParser
    {
        public static CommandArgument ParseCommand(this string cmdLine, IConsoleWriter console)
        {
            if (string.IsNullOrWhiteSpace(cmdLine)) return null;
            string[] lineParts = cmdLine.Split(new[] { ' ' });
            if (lineParts.Count()>2)
            {
                console.WriteLine("too many command line arguments. command ignored.");
                return null;
            }

            var cmd = new CommandArgument()
                          {
                              Cmd = lineParts[0],
                              Argument = lineParts.Count() == 2 ? lineParts[1] : null
                          };

            return cmd;
        }
    }

}
