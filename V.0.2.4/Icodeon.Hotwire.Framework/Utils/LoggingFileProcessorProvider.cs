using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Providers;

namespace Icodeon.Hotwire.Framework.Utils
{
    /// <summary>
    /// Logs the file process request and then simulates process file taking while by waiting diffent times on each request, between 100 and 1000ms.
    /// </summary>
    public class LoggingFileProcessorProvider : IFileProcessorProvider
    {
        private NLog.Logger _logger;

        public LoggingFileProcessorProvider(NLog.Logger logger)
        {
            _logger = logger;
        }

        public LoggingFileProcessorProvider()
        {
            _logger = NLog.LogManager.GetLogger("fileProcessor");
            _logger.Trace("new LoggingFileProcessorProvider()");
        }

        public void ProcessFile(string trackingNumber, string transaction_id, NameValueCollection requestParams)
        {
            _logger.Trace("ProcessFile(resource_file='{0}', transaction_id='{1}', requestParams=[see below])",trackingNumber, transaction_id, requestParams);
            _logger.TraceParameters(requestParams);
            _logger.Trace("Checking that '{0}' exists in the processing folder.", trackingNumber);
            string filepath = Path.Combine(HotwireFilesProvider.GetFilesProviderInstance(_logger).ProcessingFolderPath,trackingNumber);
            if (!File.Exists(filepath)) throw new FileNotFoundException("Could not find resource file '" + filepath + "' to process!");
            if (trackingNumber.Contains("_THROW_")) throw new ApplicationException("This is a test exception which was triggered because the tracking number contains '_THROW_'.");
            int processingTime = GetProcessingTime()*100; 
            _logger.Trace("simulating processing, pausing for {0}ms.", processingTime);
            Thread.Sleep(processingTime);
        }

        // just a bit of hackery so that we can simulate processing of different files
        // taking different times. 
        static int procTimePos = 0;
        private static int GetProcessingTime()
        {
            var times = new int[] { 10, 5, 1, 3 };
            return times[procTimePos++ % 3];
        }
    }
}
