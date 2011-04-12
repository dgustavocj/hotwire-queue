using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Contracts
{
    public interface IHttpResponsableWriter
    {
        string ContentType { get; set; }
        void Write(string s);
        int StatusCode { get; set; }
    }

}
