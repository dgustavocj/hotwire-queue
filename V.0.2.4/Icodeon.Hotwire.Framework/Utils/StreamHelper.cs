using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Icodeon.Hotwire.Framework.Utils
{
    public class ParsedBody
    {
        public NameValueCollection Parameters { get; set; }
        public string BodyText { get; set; }
    }


    public static class StreamHelper
    {

        public static string ReadStream(this Stream body)
        {
            using (StreamReader reader = new StreamReader(body))
            {
                String streamText = reader.ReadToEnd();
                return streamText;
            }
        }



        public static ParsedBody ParseBody(this Stream body)
        {
            var retval = new ParsedBody();
            using (StreamReader reader = new StreamReader(body))
            {
                String res = reader.ReadToEnd();
                NameValueCollection coll = HttpUtility.ParseQueryString(res, Encoding.UTF8);
                retval.BodyText = res;
                retval.Parameters = coll;
            };
            return retval;
        }
    }
}
