using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Configuration
{
    public interface IOAuthProviderConfig : IAssemblyProvider
    {
        string PartnerConsumerKey { get;  }
    }
}
