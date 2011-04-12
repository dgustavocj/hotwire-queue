using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Icodeon.Hotwire.Framework.Utils
{
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

        // if not OAUTH only do this once, otherwise twice as below.
        public static NameValueCollection ParseNameValues(this Stream body)
        {
            // if using oauth then there is some strange encoding going on with slashes that HttpUtility does not take into account?
            using (StreamReader reader = new StreamReader(body))
            {
                String res = reader.ReadToEnd();
                NameValueCollection coll = HttpUtility.ParseQueryString(res);
                foreach (var key in coll.AllKeys)
                {
                    coll[key] = HttpUtility.UrlDecode(coll[key]);
                }
                return coll;
            }
        }
    }
}
