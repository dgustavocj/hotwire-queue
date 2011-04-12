using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.MediaTypes;

namespace Icodeon.Hotwire.Framework.Contracts
{
    public interface IMediaInfo
    {
        string ContentType { get;  }
        IEnumerable<string> Extensions { get;  }
        eMediaType Type { get; }
    }
}
