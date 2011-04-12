using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Icodeon.Hotwire.Framework
{
    public class HotLogger 
    {
        private readonly Logger _logger;

        public HotLogger(Logger logger)
        {
            _logger = logger;
        }

        public void Info(string message, params object[] args)
        {
            if (_logger!=null) _logger.Info(message, args);
        }


        public void Trace(string message, params object[] args)
        {
            if(_logger!=null) _logger.Trace(message,args);
        }
    }
}
