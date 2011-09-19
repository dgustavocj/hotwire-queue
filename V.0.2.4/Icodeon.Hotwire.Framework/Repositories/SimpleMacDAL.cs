using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Providers;
//using Icodeon.Hotwire.Framework.Repository;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.DAL
{
    public class SimpleMacDal : ISimpleMacDAL, IRepository
    {

        public static string ConnectionString
        {
            get
            {
                var cStringConfig = ConfigurationManager.ConnectionStrings["hotwire.simpleMac"];
                if (cStringConfig == null) throw new ArgumentNullException("Could not find connection string setting 'hotwire.simpleMac'");
                return cStringConfig.ConnectionString;
            }
        }


        //private SimpleMacModel _db;
        public SimpleMacDal(string connectionString)
        {
           // _db = new SimpleMacModel(connectionString);
        }


        public void CacheRequest(string hotwireMac, Guid salt, string url, int msToExpire)
        {
            // do nothing! TDD

            //// need to use data time provider
            //var macSaltHistory = new MacSaltHistory()
            //                  {
            //                      Mac = hotwireMac,
            //                      Salt = salt,
            //                      Url = url.StartString(UrlMaxLength),
            //                      Expires = DateTime.Now.AddMilliseconds(msToExpire)
            //                  };
            //_db.MacSaltHistories.InsertOnSubmit(macSaltHistory);
            //_db.SubmitChanges();

        }

        public bool RequestsExists(string hotwireMac, Guid salt)
        {
            return false;
            //var exists = _db.MacSaltHistories.FirstOrDefault(msh => msh.Mac == hotwireMac && msh.Salt == salt);
            //return (exists != null);
        }

        // mucky but will do for now to get the model meta data.
        public int UrlMaxLength
        {
            get { return 50; }
        }


        public IRepository Repository
        {
            get { return this; }
        }

        void IRepository.Create()
        {
          //  _db.CreateDatabase();
        }

        void IRepository.Delete()
        {
           // _db.DeleteDatabase();
        }


        public void RemoveExpiredItems(IDateTime dateTime)
        {
            // this is where linq2sql will suffer as it doesnt support batch updates
            // if this becomes a bottleneck, then consider changing to simple.data
            // I am keeping the linq2sql model in place because linq2sql gives us the wonderful
            // if (!db.Exists()) db.CreateDatabase(); which we need for testing and for deployment
        }


        bool IRepository.Exists()
        {
            return true;
            // return _db.DatabaseExists();
        }
    }
}
