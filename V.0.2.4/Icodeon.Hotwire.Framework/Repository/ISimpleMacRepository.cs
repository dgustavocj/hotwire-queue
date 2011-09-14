using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Repository
{
    public interface ISimpleMacRepository
    {
        void RecordRequest(string hotwireMac, Guid salt, string startString);
        bool RequestsExists(string hotwireMac, Guid salt);
        int UrlMaxLength { get; }
        IRepository Database { get; }
    }

    // mostly needed for testing
    public interface IRepository
    {
        void CreateIfNotExists();
        void Delete();
    }
     
}
