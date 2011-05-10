using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Diagnostics;
using NLog;

namespace Icodeon.Hotwire.Framework
{
    public class HotLogger : LoggerBase
    {
        private readonly Logger _logger;
        public HotLogger(Logger logger)
        {
            _logger = logger;
        }

        public override void Debug(string message, params object[] parameters)
        {
            if (_logger == null) return;
            _logger.Debug(message, parameters);
        }

        public override void Warn(string message, params object[] parameters)
        {
            if (_logger == null) return;
            _logger.Warn(message, parameters);
        }

        public override void Fatal(string message, params object[] parameters)
        {
            if (_logger == null) return;
            _logger.Fatal(message,parameters);
        }

        public override void Trace(string message, params object[] parameters)
        {
            if (_logger == null) return;
            _logger.Trace(message, parameters);
        }

        public override void ErrorException(string message, Exception ex)
        {
            if (_logger == null) return;
            _logger.ErrorException(message, ex);
        }


        public override void FatalException(string message, Exception ex)
        {
            if (_logger == null) return;
            _logger.FatalException(message, ex);
        }

        public override void Info(string message, params object[] parameters)
        {
            if (_logger == null) return;
            _logger.Trace(message, parameters);
        }


        public override void TraceParameters(NameValueCollection coll)
        {
            if (_logger == null) return;
            foreach (var key in coll.AllKeys)
            {
                _logger.Trace("{0,-30}:{1}", key, coll[key]);
            }
        }

        public override void Error(string message, params object[] parameters)
        {
            if (_logger == null) return;
            _logger.Error(message, parameters);
        }

        public override void LogMethodCall(string methodName, params object[] parameters)
        {
            if (_logger == null) return;
            _logger.Trace(methodName);
            _logger.Trace("-----------");
            foreach (var parameter in parameters)
            {
                if (parameter == null)
                {
                    _logger.Trace("\t{0,-35}{1}", "[nullparameter]", "[NULL]");
                }
                else
                {
                    _logger.Trace("\t{0,-35}Not null.", parameter.GetType().ToString());
                }
            }
        }

        private static HotLogger _nullLogger;
        public static HotLogger NullLogger
        {
            get { return _nullLogger ?? (_nullLogger = new HotLogger(null)); }
        }


    }
}
