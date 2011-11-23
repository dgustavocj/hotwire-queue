using System;
using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Providers;
using NLog;

namespace Icodeon.Hotwire.TestFramework
{
    /// <summary>
    /// AcceptanceTest class uses filesProvider and therefore requires that you completely define all the hotwire folders in your app.config, otherwise you will get null reference.
    /// </summary>
    public class FilesProviderAcceptanceTest : AcceptanceTest 
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public HotwireFilesProvider FilesProvider { get; private set; }
        public FilesProviderAcceptanceTest() 
        {
            _logger.Trace("cxtor FilesProviderAcceptanceTest()");
            // ADH: 23.11.2011 wrapping fixture class constructor code in try catch because test runners (Team City, NUnit) can't report on errors in the fixture setups
            try
            {
                FilesProvider = HotwireFilesProvider.GetFilesProviderInstance();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
                throw;
            }
        }
    }
}
