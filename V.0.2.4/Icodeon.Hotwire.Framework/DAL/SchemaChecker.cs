using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Icodeon.Hotwire.Framework.Diagnostics;
using NLog;

namespace Icodeon.Hotwire.Framework.DAL
{
    public class SchemaChecker
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly string _connectionString;

        public SchemaChecker(string connectionString)
        {
            _connectionString = connectionString;
            _logger.Trace("connection string:{0}",_connectionString);
        }


        public SchemaChecker ClearoutAnyTestData()
        {
            using (var db = new HotwireContext(_connectionString))
            {
                int cntHistories = db.SimpleMacHistories.Count();
                _logger.Trace("{0} simplemac histories.",cntHistories);
                if (cntHistories>0)
                {
                    _logger.Trace("deleting {0} simplemac histories.",cntHistories);
                    db.SimpleMacHistories.DeleteAllOnSubmit(db.SimpleMacHistories);
                    db.SubmitChanges();
                }
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
                    _logger.Trace("starting transaction scope (locking db)");
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
                        _logger.Trace("writing test history record to db.");
                        db.SimpleMacHistories.InsertOnSubmit(history);
                        db.SubmitChanges();
                    }

                    using (var db = new HotwireContext(ConnectionStringManager.HotwireConnectionString))
                    {
                        // quick check of the schema:
                        _logger.Trace("reading test history record from db.");
                        var history2 = db.SimpleMacHistories.FirstOrDefault(h => h.Salt == history.Salt);

                        history2.Ensure(    h => history2 != null,
                                            h => h.Created == history.Created,
                                            h => h.Expires == history.Expires,
                                            h => h.Salt == history.Salt,
                                            h => h.Url == history.Url);
                        _logger.Trace("validated.");
                    }
                    // rollback so that db is not affected.
                    scope.Dispose();
                    _logger.Trace("Rolled back records. (released lock on db)");
                }
                return this;
            }
            catch (Exception ex)
            {
                var msg = string.Format("Error quick check validating hotwire database schema. {0}", ex);
                _logger.FatalException(msg,ex);
                throw new Exception(msg);
            }
            
        }
    }
}
