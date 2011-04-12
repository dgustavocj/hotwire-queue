using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Events
{
    public class EnqueueRequestEventArgs : EventArgs
    {
        public EnqueueRequestDTO EnqueueRequest { get; private set; }

        public EnqueueRequestEventArgs(EnqueueRequestDTO dto)
        {
            EnqueueRequest = dto;
        }
    }
}
