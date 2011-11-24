using NLog;

namespace Icodeon.Hotwire.Framework.Diagnostics
{
    public class LoggingErrorHandler : IExceptionHandler
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public LoggingErrorHandler()
        {

        }

        public void HandleException(object sender, ExceptionEventArgs args)
        {
            _logger.Error("HandleException() //");
            _logger.ErrorException(args.Exception.Message,args.Exception);
            _logger.Error(args.Exception.GetType().ToString());
            _logger.Error("Request : {0}", args.Request);
            _logger.Error("Section: {0}", args.Section);
            _logger.Error("// HandleException()");
        }
    }
}