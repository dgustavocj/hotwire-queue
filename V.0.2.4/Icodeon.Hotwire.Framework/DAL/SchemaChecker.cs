using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Icodeon.Hotwire.Framework.Diagnostics;

namespace Icodeon.Hotwire.Framework.DAL
{
    public class SchemaChecker
    {
        private readonly string _connectionString;

        public SchemaChecker(string connectionString)
        {
            _connectionString = connectionString;
        }


        public SchemaChecker ClearoutAnyTestData()
        {
            using (var db = new HotwireContext(_connectionString))
            {
                db.SimpleMacHistories.DeleteAllOnSubmit(db.SimpleMacHistories);
                db.SubmitChanges();
            }
            return this;

        }

        public SchemaChecker CheckSchemaThrowExceptionIfInvalid()
        {
            try
            {

                // simply check the fingerprint of the schema, to make sure it's suitable for our tests.
                using (TransactionScope scope = new TransactionScope())
                {
                    var history = new SimpleMacHistory()
                    {
                        // can't use DateTime.Now because when that get's saved to DB some precision is lost, and comparing later will fail, 
                        // as they will not be exactly the same, unless we specify a specific time that has a trusted precision.
                        Created = new DateTime(2011, 12, 12, 12, 12, 12),
                        Expires = new DateTime(2011, 12, 13, 12, 12, 12),
                        Salt = Guid.NewGuid(),
                        Url = "http://localhost/test.com"
                    };
                    using (var db = new HotwireContext(_connectionString))
                    {
                        db.SimpleMacHistories.InsertOnSubmit(history);
                        db.SubmitChanges();
                    }

                    using (var db = new HotwireContext(ConnectionStringManager.HotwireConnectionString))
                    {
                        // quick check of the schema:
                        var history2 = db.SimpleMacHistories.FirstOrDefault(h => h.Salt == history.Salt);

                        history2.Ensure(    h => history2 != null,
                                            h => h.Created == history.Created,
                                            h => h.Expires == history.Expires,
                                            h => h.Salt == history.Salt,
                                            h => h.Url == history.Url);
                    }
                    // rollback so that db is not affected.
                    scope.Dispose();
                }
                return this;
            }
            catch (Exception ex)
            {
                throw new Exception("Error quick check validating hotwire database schema. " + ex);
            }
            
        }
    }
}
