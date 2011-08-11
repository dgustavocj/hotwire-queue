using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Events;
using Icodeon.Hotwire.Framework.Modules;

namespace Icodeon.Hotwire.Framework.Diagnostics
{
    public class ExceptionEventArgs : EnqueueRequestEventArgs
    {
        public HttpRequestContext Request { get; set; }
        public Exception Exception { get; set; }
        public ePipeLineSection Section { get; set; }

        // I have chosen not to raise two different events with different event args because I want a very simple
        // way for someone to be able to write a single handler that can be used across all event types
        // I'm sure I can refactor this to a hierarchy and a handler can process any of the base types, but will look into refactoring this 
        // later. For now, single exception type.

        /// <param name="dto">Caller must check if dto is null before using.</param>
        /// <param name="request">Caller must check if request is null before using.</param>
        public ExceptionEventArgs(Exception exception, ePipeLineSection section, EnqueueRequestDTO dto, HttpRequestContext request) : base(dto)
        {
            Request = request;
            Exception = exception;
            Section = section;
        }
    }
}
