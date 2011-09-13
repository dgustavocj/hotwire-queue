using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Utils
{
    public class DateTimeWrapper : IDateTime
    {
        public int SecondsSince1970
        {
            get
            {
                TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
                int serverTimeStamp = (int) timeSpan.TotalSeconds;
                return serverTimeStamp;
            }
        }
    }
}
