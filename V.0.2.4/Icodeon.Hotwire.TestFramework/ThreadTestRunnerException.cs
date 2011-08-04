using System;
using System.Collections.Generic;

namespace Icodeon.Hotwire.TestFramework
{
    public class ThreadTestRunnerException : ApplicationException
    {
        public List<Exception> Exceptions { get; set; }
        public Exception LastException { get; set; }
        public ThreadTestRunnerException(Exception lastException, List<Exception> exceptions)
            : base("ThreadTestRunnerException see LastException and Exceptions for the specific exception/s.")
        {
            LastException = lastException;
            Exceptions = exceptions;
        }
    }
}