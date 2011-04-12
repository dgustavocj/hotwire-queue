using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Contracts
{
    public interface IHotwireServiceEvents
    {
        event EventHandler<ServiceRequestEventArgs> ServiceRequesting;
        event EventHandler<EnqueuingEventArgs> Enqueuing;
    }
}