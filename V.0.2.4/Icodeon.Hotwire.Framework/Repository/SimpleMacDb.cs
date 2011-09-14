using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.Framework.Repository
{
    public class SimpleMacDb : ISimpleMacRepository, IRepository
    {
        private SimpleMacModelDataContext _db;
        public SimpleMacDb(string connectionString)
        {
            _db = new SimpleMacModelDataContext(connectionString);
        }


        public void RecordRequest(string hotwireMac, Guid salt, string url)
        {
            var macSaltHistory= new MacSaltHistory()
                              {
                                  Mac = hotwireMac,
                                  Salt = salt,
                                  Url = url.StartString(UrlMaxLength)
                              };
            _db.MacSaltHistories.InsertOnSubmit(macSaltHistory);
            _db.SubmitChanges();
        }

        public bool RequestsExists(string hotwireMac, Guid salt)
        {
            var exists = _db.MacSaltHistories.FirstOrDefault(msh => msh.Mac == hotwireMac && msh.Salt == salt);
            return (exists != null);
        }

        // mucky but will do for now to get the model meta data.
        public int UrlMaxLength
        {
            get { return 50; }
        }


        public IRepository Database
        {
            get { return this; }
        }

        void IRepository.CreateIfNotExists()
        {
            if (!_db.DatabaseExists()) _db.CreateDatabase();
        }

        void IRepository.Delete()
        {
            if (_db.DatabaseExists()) _db.DeleteDatabase();
        }
    }
}
