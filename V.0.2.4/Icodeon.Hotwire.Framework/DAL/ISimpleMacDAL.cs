using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.DAL.Params;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.DAL
{
    public interface ISimpleMacDAL
    {
        void CacheRequest(CacheRequestParams cacheRequestParams);
        bool RequestsExists(Guid salt);
        void RemoveExpiredItems(IDateTime dateTime);
        int UrlMaxLength { get; }
    }

}
