using Icodeon.Hotwire.Framework;
using Icodeon.Hotwire.Framework.Providers;

namespace Icodeon.Hotwire.TestFramework
{
    /// <summary>
    /// AcceptanceTest class uses filesProvider and therefore requires that you completely define all the hotwire folders in your app.config, otherwise you will get null reference.
    /// </summary>
    public class FilesProviderAcceptanceTest : AcceptanceTest 
    {
        public HotwireFilesProvider FilesProvider { get; private set; }
        public FilesProviderAcceptanceTest() 
        {
            FilesProvider = HotwireFilesProvider.GetFilesProviderInstance();

        }
    }
}
