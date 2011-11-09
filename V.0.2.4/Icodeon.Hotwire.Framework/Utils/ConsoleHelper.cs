using System;

namespace Icodeon.Hotwire.Framework.Utils
{
    public static class ConsoleHelper
    {

        public static void WriteLineColor(this IConsoleWriter console, string message,  ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            var current = console.GetCurrentColours();
            if (background!=null) console.BackgroundColor = background.Value;
            if (foreground != null) console.ForegroundColor = foreground.Value;
            console.WriteLine(message);
            console.ResetColor(current);
        }

        public static void WriteColor(this IConsoleWriter console, string message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            var current = console.GetCurrentColours();
            if (background != null) console.BackgroundColor = background.Value;
            if (foreground != null) console.ForegroundColor = foreground.Value;
            console.Write(message);
            console.ResetColor(current);
        }


        public static ColorDTO GetCurrentColours(this IConsoleWriter console)
        {
            return new ColorDTO
                       {
                           BackgroundColor = console.BackgroundColor,
                           ForegroundColor = console.ForegroundColor
                       };
        }

        public static void ResetColor(this IConsoleWriter console, ColorDTO color)
        {
            console.ForegroundColor = color.ForegroundColor;
            console.BackgroundColor = color.BackgroundColor;
        }

        public static void WriteBold(this IConsoleWriter console, string message)
        {
            console.WriteColor(message, ConsoleColor.Yellow, ConsoleColor.Black);
        }

        public static void WriteError(this IConsoleWriter console,string message)
        {
            console.WriteLineColor(message, ConsoleColor.Red, ConsoleColor.White);
        }
    }
}