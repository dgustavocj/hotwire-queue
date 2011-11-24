using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework
{
    public class HotLogger : LoggerBase
    {
        private readonly Logger _logger;
        
        public HotLogger(Logger logger) : this(logger,true) { }

        public HotLogger(Logger logger, bool echoToConsole) : base(echoToConsole)
        {
            _logger = logger;
        }


        public override void Debug(string message, params object[] parameters)
        {
            if (_logger == null) return;
            _logger.Debug(message, parameters);
            if (EchoToConsole) Console.WriteLine(message, parameters);
        }

        public override void Warn(string message, params object[] parameters)
        {
            if (_logger == null) return;
            _logger.Warn(message, parameters);
            if (EchoToConsole) Console.WriteLine(message, parameters);
        }

        public override void Fatal(string message, params object[] parameters)
        {
            if (_logger == null) return;
            _logger.Fatal(message, parameters);
            if (EchoToConsole) Console.WriteLine(message, parameters);
        }

        public override void Trace(string message, params object[] parameters)
        {
            // add a prompt to the front of the trace message, so that we can 
            // easily identify "action summaries" from detailed code trace statements.
            message = "> " + message;
            if (_logger == null) return;
            _logger.Trace(message, parameters);
            if (EchoToConsole) Console.WriteLine(message, parameters);
        }

        public override void TraceTitle(string message, params object[] parameters)
        {
            if (_logger == null) return;
            string text = string.Format(message, parameters);
            _logger.Trace("");
            _logger.Trace(text);
            _logger.Trace(text.UnderlineText('-'));
            // Take a look at using logging rules to echo stuff to console instead of having to re-invent the wheel.
            if (EchoToConsole)
            {
                Console.WriteLine(message, parameters);
                Console.WriteLine(message.UnderlineText('-'));
            }
        }


        public override void ErrorException(string message, Exception ex)
        {
            if (_logger == null) return;
            _logger.ErrorException(message, ex);
            if (EchoToConsole) Console.WriteLine("EXCEPTION : " + message + " " + ex.Message);
        }


        public override void FatalException(string message, Exception ex)
        {
            if (_logger == null) return;
            _logger.FatalException(message, ex);
            if (EchoToConsole) Console.WriteLine("`FATAL EXCEPTION : " + message + " " + ex.Message);
        }

        public override void Info(string message, params object[] parameters)
        {
            if (_logger == null) return;
            _logger.Trace(message, parameters);
            if (EchoToConsole) Console.WriteLine(message, parameters);
        }


        public override void TraceParameters(NameValueCollection coll)
        {
            if (_logger == null) return;
            foreach (var key in coll.AllKeys)
            {
                _logger.Trace("{0,-30}:{1}", key, coll[key]);
                if (EchoToConsole) Console.WriteLine("{0,-30}:{1}", key, coll[key]);
            }
        }

        public override void Error(string message, params object[] parameters)
        {
            if (_logger == null) return;
            _logger.Error(message, parameters);
            if (EchoToConsole) Console.WriteLine("Error - " + message, parameters);

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

        public static bool LoggingEnabled
        {
            get { return bool.Parse(ConfigurationManager.AppSettings["logging"] ?? "true" ); }
        }

        private static HotLogger _instance;

        /// <summary>
        /// Use GetCurrentClassLogger in unit tests, and use GetLogger in production code to get a specific "named" logger/ GetCurrentClassLogger uses reflection and is slow, GetLogger is quicker.
        /// </summary>
        /// <returns></returns>
        public static HotLogger GetCurrentClassLogger()
        {
            return _instance ?? (_instance = LoggingEnabled ? new HotLogger(LogManager.GetCurrentClassLogger() ) : new HotLogger(null));
        }

        /// <summary>
        /// Use GetCurrentClassLogger in unit tests, and use GetLogger in production code to get a specific "named" logger/ GetCurrentClassLogger uses reflection and is slow, GetLogger is quicker.
        /// </summary>
        /// <returns></returns>
        public static HotLogger GetLogger(string logger)
        {
            return _instance ?? (_instance = LoggingEnabled ? new HotLogger(LogManager.GetLogger(logger)) : new HotLogger(null));
        }



    }
}
