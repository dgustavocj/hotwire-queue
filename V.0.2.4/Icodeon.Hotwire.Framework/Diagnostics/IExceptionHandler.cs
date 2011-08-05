using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Configuration;

namespace Icodeon.Hotwire.Framework.Diagnostics
{
    public interface IExceptionHandler
    {
        void HandleException(Exception ex, ePipeLineSection section);
    }
}
