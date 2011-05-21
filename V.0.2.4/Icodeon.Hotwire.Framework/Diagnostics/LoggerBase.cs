using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Diagnostics
{
    // ADH: class perhaps should be called NullableLoggerBase, however I don't want to have NullableLoggerBase as the field type
    //      that will appear in almost every class.

    // ADH: this is an abstract base class instead of an interface so that if we add methods to new derived classes we won't break existing implementations.
    //      see 

    public abstract class LoggerBase
    {
        protected LoggerBase(bool echoToConsole)
        {
            EchoToConsole = echoToConsole;
        }

        public abstract void FatalException(string message, Exception ex);
        public abstract void ErrorException(string message, Exception ex);
        public abstract void TraceTitle(string message, params object[] parameters);

        public abstract void Trace(string message, params object[] parameters);
        public abstract void Debug(string message, params object[] parameters);
        public abstract void Error(string message, params object[] parameters);
        public abstract void Info(string message, params object[] parameters);
        public abstract void Warn(string message, params object[] parameters);
        public abstract void Fatal(string message, params object[] parameters);

        

        public abstract void TraceParameters(NameValueCollection coll);
        public abstract void LogMethodCall(string methodName, params object[] parameters);
        
        public bool EchoToConsole { get; set; }
    }
}
