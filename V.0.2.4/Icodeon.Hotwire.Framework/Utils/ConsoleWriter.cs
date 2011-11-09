using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Utils
{

    public interface IConsoleWriter
    {
        void WriteLine(string format, params object[] arg);
        void WriteLine(string format);
        void Write(string value);
        string ReadLine();
        void Clear();
        ConsoleColor ForegroundColor { get; set; }
        ConsoleColor BackgroundColor { get; set; }
    }

    public class ConsoleWriter : IConsoleWriter
    {
        // need a lock because race conditions cause colours not to reset properly, since we call getColors, change color, write, then reset back again!
        private Object locker = new Object();

        public void WriteLine(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);
        }

        public void WriteColor(string message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            lock(locker)
            {
                var current = GetCurrentColours();
                if (background != null) Console.BackgroundColor = background.Value;
                if (foreground != null) Console.ForegroundColor = foreground.Value;
                Write(message);
                this.ResetColor(current);
            }
        }

        public void WriteLineColor(string message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            lock(locker)
            {
                var current = GetCurrentColours();
                if (background != null) Console.BackgroundColor = background.Value;
                if (foreground != null) Console.ForegroundColor = foreground.Value;
                Console.WriteLine(message);
                this.ResetColor(current);
            }
        }



        public void WriteBold(string message)
        {
            WriteColor(message, ConsoleColor.Yellow, ConsoleColor.Black);
        }


        public static ColorDTO GetCurrentColours()
        {
            return new ColorDTO
            {
                BackgroundColor = Console.BackgroundColor,
                ForegroundColor = Console.ForegroundColor
            };
        }

        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        public void Write(string value)
        {
            Console.Write(value);
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void Write(string format, params object[] arg)
        {
            Console.Write(format, arg);
        }



        public string ReadLine()
        {
            return Console.ReadLine();
        }


        public ConsoleColor ForegroundColor
        {
            get { return Console.ForegroundColor; }
            set { Console.ForegroundColor = value; }
        }

        public ConsoleColor BackgroundColor
        {
            get { return Console.BackgroundColor; }
            set { Console.BackgroundColor = value; }
        }
    }
}
