using System;
using System.Linq;

namespace Icodeon.Hotwire.Framework.Utils
{
    public static class StringExtensions
    {
        public static string StartString(this string src, int numChars)
        {
            if (src.Length >= numChars) return src.Substring(0, numChars);
            return src;
        }

        public static bool EqualsIgnoreCase(this string src, string compareTo)
        {
            return src.Equals(compareTo, StringComparison.OrdinalIgnoreCase);
        }

        public static string SecondLast(this string[] src)
        {
            return src[src.Length - 2];
        }

        public static string IncrementNumberAtEndOfString(this string src)
        {
            // has this already been inc'd?
            var lastChars = src.SplitDot().LastOrDefault();
            if (lastChars==null)
            {
                return src + ".01";
            }
            else
            {
                int lastNum;
                bool isNum = int.TryParse(lastChars, out lastNum);
                if (!isNum) return src + ".01";
                return src.Substring(0, src.Length - lastChars.Length) + (++lastNum);
            }
        }


        public static string ThirdLast(this string[] src)
        {
            return src[src.Length - 3];
        }

        public static string[] SplitDot(this string src)
        {
            return src.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        }


        public static string[] SplitComma(this string src)
        {
            return src.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
        }

    }
}
