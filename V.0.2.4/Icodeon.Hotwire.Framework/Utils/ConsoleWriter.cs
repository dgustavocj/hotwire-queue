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

        void Log(string value);
        void Log(string format, params object[] args);
        void LogError(string value);
        void LogError(string format, params object[] args);
        void LogBold(string value);
        void LogBold(string first, string bold, string last);
        void LogBold(string format, params object[] args);
        void WriteTime();
        
        ConsoleColor ForegroundColor { get; set; }
        ConsoleColor BackgroundColor { get; set; }
    }

    public class ConsoleWriter : IConsoleWriter
    {
        private readonly IDateTime _dateTime;
        // need a lock because race conditions cause colours not to reset properly, since we call getColors, change color, write, then reset back again!
        private Object locker = new Object();

        public ConsoleWriter(IDateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public ConsoleWriter()
        {
            _dateTime = new DateTimeWrapper();
        }


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



        public virtual string ReadLine()
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

        public void Log(string text)
        {
            ConsoleHelper.Log(this,_dateTime, text);
        }

        public void Log(string format, params object[] args)
        {
            ConsoleHelper.Log(this,_dateTime, format, args);
        }

        public void LogError(string value)
        {
            ConsoleHelper.LogError(this,_dateTime, value);
        }

        public void LogError(string format, params object[] args)
        {
            ConsoleHelper.LogError(this,_dateTime, format, args);
        }

        public void LogBold(string value)
        {
            ConsoleHelper.LogBold(this,_dateTime, value);
        }

        public void LogBold(string format, params object[] args)
        {
            ConsoleHelper.LogBold(this,_dateTime, format, args);
        }

        public void LogBold(string first, string bold, string last)
        {
            ConsoleHelper.LogFirstBoldLast(this,_dateTime, first, bold, last);
        }

        public void WriteTime()
        {
            ConsoleHelper.WriteTime(this,_dateTime);
        }
    }
}
