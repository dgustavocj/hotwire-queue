using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Icodeon.Hotwire.Framework.Utils
{
    public class BodyParsed
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



        // if not OAUTH only do this once, otherwise twice as below.
        //public static BodyParsed ParseBody(this Stream body, bool isOAuth)
        public static BodyParsed ParseBody(this Stream body)
        {
            
            using (StreamReader reader = new StreamReader(body))
            {
                String res = reader.ReadToEnd();
                NameValueCollection coll = HttpUtility.ParseQueryString(res);

                // if using oauth then there is some strange encoding going on with slashes that HttpUtility does not take into account? so urldecode?
                //if (isOAuth)
                //{
                //    foreach (var key in coll.AllKeys)
                //    {
                //        coll[key] = HttpUtility.UrlDecode(coll[key]);
                //    }
                //}
                var retval = new BodyParsed()
                {
                    BodyText = res,
                    Parameters = coll
                };

                return retval;
            }
        }
    }
}
