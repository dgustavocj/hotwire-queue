using System;

namespace Icodeon.Hotwire.Framework.Utils
{
    public static class ConsoleHelper
    {
        public static Object locker = new Object();

        public static void WriteLineColor(this IConsoleWriter console, string message,  ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            lock (locker)
            {
                var current = console.GetCurrentColours();
                if (background != null) console.BackgroundColor = background.Value;
                if (foreground != null) console.ForegroundColor = foreground.Value;
                console.WriteLine(message);
                console.ResetColor(current);
            }
        }

        public static void WriteColor(this IConsoleWriter console, string message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            lock(locker)
            {
                var current = console.GetCurrentColours();
                if (background != null) console.BackgroundColor = background.Value;
                if (foreground != null) console.ForegroundColor = foreground.Value;
                console.Write(message);
                console.ResetColor(current);
            }
        }


        public static ColorDTO GetCurrentColours(this IConsoleWriter console)
        {
            lock(locker)
            {
                return new ColorDTO
                {
                    BackgroundColor = console.BackgroundColor,
                    ForegroundColor = console.ForegroundColor
                };
            }
        }

        public static void ResetColor(this IConsoleWriter console, ColorDTO color)
        {
            lock(locker)
            {
                console.ForegroundColor = color.ForegroundColor;
                console.BackgroundColor = color.BackgroundColor;
            }
        }


        public static void WriteBold(this IConsoleWriter console, string message)
        {
            console.WriteColor(message, ConsoleColor.Cyan, ConsoleColor.Black);
        }


        public static void WriteLineError(this IConsoleWriter console,string message)
        {
            console.WriteLineColor(message, ConsoleColor.Red, ConsoleColor.Black);
        }

        public static void LogBold(this IConsoleWriter console, IDateTime dateTime, string value)
        {
            WriteTime(console, dateTime);
            console.WriteBold(value + "\n");
            
        }

        public static void LogBold(this IConsoleWriter console, IDateTime dateTime, string format, params object[] args)
        {
            WriteTime(console, dateTime);
            console.WriteBold(string.Format(format, args) + "\n");
        }

        public static void Log(this IConsoleWriter console, IDateTime dateTime, string format, params object[] args)
        {
            WriteTime(console, dateTime);
            console.WriteLine(string.Format(format,args));
        }

        public static void LogError(this IConsoleWriter console, IDateTime dateTime, string value)
        {
            WriteTime(console, dateTime);
            console.WriteLineError(value);
        }

        public static void LogError(this IConsoleWriter console, IDateTime dateTime, string format, params object[] args)
        {
            WriteTime(console, dateTime);
            console.WriteLineError(string.Format(format, args));
        }

        public static void WriteTime(this IConsoleWriter console, IDateTime dateTime)
        {
            console.WriteColor(dateTime.ToUtcTime() + " ", ConsoleColor.DarkGray);
        }

    }
}