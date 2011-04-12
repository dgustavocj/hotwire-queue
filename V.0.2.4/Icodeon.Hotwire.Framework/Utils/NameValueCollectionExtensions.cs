using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.DTOs;

namespace Icodeon.Hotwire.Framework
{
    public static class NameValueCollectionExtensions
    {
        public static List<NameValueDTO> ToNameValues(this NameValueCollection src)
        {
            var nameValues = new List<NameValueDTO>();
            foreach (var key in src.AllKeys)
            {
                nameValues.Add(new NameValueDTO() {Name = key, Value = src[key]});
            }
            return nameValues;
        }

        public static string Value(this List<NameValueDTO> src, string key)
        {
            var nameValue = src.FirstOrDefault(nv => nv.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (nameValue==null) throw new ArgumentNullException("Could not find item with key='" + key + "' in the NameValueDTO collection.");
            return nameValue.Value;
        }

        public static string ToTraceString(this NameValueCollection src)
        {
            var sb = new StringBuilder();
            foreach (var key in src.AllKeys)
            {
                sb.Append(string.Format("{0}={1},", key, src[key]));
            }
            return sb.ToString().TrimEnd(new[] {','});
        }
    }
}
