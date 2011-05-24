using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Providers;

namespace Icodeon.Hotwire.Tests.Framework
{
    public class FilesProviderUnitTest : UnitTest
    {
        public HotwireFilesProvider FilesProvider { get; private set; }

        public FilesProviderUnitTest() 
        {
            FilesProvider = HotwireFilesProvider.GetFilesProviderInstance(HotLogger.NullLogger);
        }



    }
}
