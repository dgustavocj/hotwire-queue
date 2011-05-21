using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Providers;

namespace Icodeon.Hotwire.Tests.Framework
{
    public class UnitTest
    {

        public HotwireFilesProvider FilesProvider { get; private set; }
        public HotLogger Logger { get; private set; }

        public UnitTest()
        {
            Logger = HotLogger.GetLogger(LogFiles.UnitTests);
            Logger.Trace("UnitTest constructor. (base class for most unit tests.)");
            FilesProvider = HotwireFilesProvider.GetFilesProviderInstance(HotLogger.NullLogger);
        }



    }
}
