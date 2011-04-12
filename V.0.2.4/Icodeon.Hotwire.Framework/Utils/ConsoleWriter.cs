using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Utils
{

    public interface IConsoleWriter
    {
        void WriteLine(string format, params object[] arg);
        void Write(string format, params object[] arg);
        string ReadLine();
    }

    public class ConsoleWriter : IConsoleWriter
    {
        public static IConsoleWriter CreateWriter()
        {
            return new ConsoleWriter();
        }

        public void WriteLine(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);
        }

        public void Write(string format, params object[] arg)
        {
            Console.Write(format, arg);
        }



        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}
