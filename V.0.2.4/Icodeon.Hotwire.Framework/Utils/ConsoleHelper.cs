using System;

namespace Icodeon.Hotwire.Framework.Utils
{
    public static class ConsoleHelper
    {

        public static void WriteLineColor(string message,  ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            var current = GetCurrentColours();
            if (background!=null) Console.BackgroundColor = background.Value;
            if (foreground != null) Console.ForegroundColor = foreground.Value;
            Console.WriteLine(message);
            ResetColor(current);
        }

        public static void WriteColor(string message, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            var current = GetCurrentColours();
            if (background != null) Console.BackgroundColor = background.Value;
            if (foreground != null) Console.ForegroundColor = foreground.Value;
            Console.Write(message);
            ResetColor(current);
        }


        public static ColorDTO GetCurrentColours()
        {
            return new ColorDTO
                       {
                           BackgroundColor = Console.BackgroundColor,
                           ForegroundColor = Console.ForegroundColor
                       };
        }

        public static void ResetColor(ColorDTO color)
        {
            Console.ForegroundColor = color.ForegroundColor;
            Console.BackgroundColor = color.BackgroundColor;
        }

        public static void WriteBold(string message)
        {
            WriteColor(message, ConsoleColor.Yellow, ConsoleColor.Black);
        }

        public static void WriteError(string message)
        {
            WriteLineColor(message, ConsoleColor.Red, ConsoleColor.White);
        }
    }
}