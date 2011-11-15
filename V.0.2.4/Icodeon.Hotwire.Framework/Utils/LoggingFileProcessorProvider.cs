using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Providers;
using NLog;

namespace Icodeon.Hotwire.Framework.Utils
{

    public class LoggingFileProcessorProvider : IFileProcessorProvider
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public LoggingFileProcessorProvider()
        {
            _logger.Trace("LoggingFileProcessorProvider()");
        }

        // ADH: 15.11.2011 removed the semi random (sleeps) as it wasn't making the test any more reliable, just slower.
        public void ProcessFile(string trackingNumber, string transaction_id, NameValueCollection requestParams)
        {
            _logger.Debug("ProcessFile(string trackingNumber, string transaction_id, NameValueCollection requestParams)");
            _logger.Info("trackingNumber:'{0}',transaction_id:'{1}'",trackingNumber, transaction_id);
            _logger.TraceParameters(requestParams);
            _logger.Trace("Checking that '{0}' exists in the processing folder.", trackingNumber);
            string filepath = Path.Combine(HotwireFilesProvider.GetFilesProviderInstance().ProcessingFolderPath,trackingNumber);
            if (!File.Exists(filepath)) throw new FileNotFoundException("Could not find resource file '" + filepath + "' to process!");
            if (trackingNumber.Contains("_THROW_")) throw new ApplicationException("This is a test exception which was triggered because the tracking number contains '_THROW_'.");
        }
        
    }
}
