using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Repositories
{
    public interface ISimpleMacDAL
    {
        void CacheRequest(string hotwireMac, Guid salt, string startString, int msToExpire);
        bool RequestsExists(string hotwireMac, Guid salt);
        void RemoveExpiredItems(IDateTime dateTime);
        int UrlMaxLength { get; }
        IRepository Repository { get; }
    }

}
