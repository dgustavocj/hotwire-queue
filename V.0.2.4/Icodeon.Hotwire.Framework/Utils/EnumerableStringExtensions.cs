using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Utils
{
    public static class EnumerableStringExtensions
    {
        public static string ToVisualizerString(this IEnumerable<string> src)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var line in src)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }


    }
}
