using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.DAL.Params;
using Icodeon.Hotwire.Framework.Providers;
//using Icodeon.Hotwire.Framework.Repository;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.DAL
{
    public class SimpleMacDal : ISimpleMacDAL
    {

        private HotwireContext _db;
        private IDateTime _dateTime;
            
        public SimpleMacDal(IDateTime dateTime, HotwireContext context)
        {
           //_db = new HotwireContext(connectionString);
            _db = context;
           _dateTime = dateTime;
        }


        public void CacheRequest(CacheRequestParams cacheRequestParams)
        {
            var simpleMacHistory = new SimpleMacHistory()
                              {
                                  Salt = cacheRequestParams.Salt,
                                  Created = _dateTime.Now,
                                  Url = cacheRequestParams.Url.StartString(UrlMaxLength),
                                  Expires = _dateTime.Now.AddMilliseconds(cacheRequestParams.MsToExpire)
                              };
            _db.SimpleMacHistories.InsertOnSubmit(simpleMacHistory);
            _db.SubmitChanges();
        }

        public bool RequestsExists(Guid salt)
        {
            //TODO: get the sql below
            var exists = _db.SimpleMacHistories.FirstOrDefault(msh => msh.Salt.Equals(salt));
            return exists != null;
        }

        // mucky but will do for now to get the model meta data.
        public int UrlMaxLength
        {
            get { return 200; }
        }


        public void RemoveExpiredItems(IDateTime dateTime)
        {
            // this is where linq2sql will suffer as it doesnt support batch updates
            // if this becomes a bottleneck, then consider changing to simple.data
            
            var expiredItems = _db.SimpleMacHistories.Where(h => h.Expires < _dateTime.Now).ToList();
            _db.SimpleMacHistories.DeleteAllOnSubmit(expiredItems);
        }

    }
}
