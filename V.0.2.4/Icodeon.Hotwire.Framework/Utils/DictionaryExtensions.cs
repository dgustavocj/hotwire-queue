using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Utils
{
    public static class DictionaryExtensions
    {
        public static  Dictionary<string,T> ForEach<T>(this Dictionary<string,T> src, Action<string,T> action)
        {
            src.Keys.ToList().ForEach( key => action(key, src[key]));
            return src;
        }

    }
}
