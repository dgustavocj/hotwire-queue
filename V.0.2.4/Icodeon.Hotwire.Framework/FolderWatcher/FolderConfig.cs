
namespace Icodeon.Hotwire.Framework.FolderWatcher
{
    public class FolderConfig
    {
        public string Key { get; set; }
        public string FolderWatcherScriptTypeNameCommaAssembly { get; set; }
        public string FolderPath { get; set; }
        public string FileMatchPattern { get; set; }

        public FolderConfig(string key, string folderWatcherScriptTypeNameCommaAssembly, string folderPath, string fileMatchPattern)
        {
            Key = key;
            FolderWatcherScriptTypeNameCommaAssembly = folderWatcherScriptTypeNameCommaAssembly;
            FolderPath = folderPath;
            FileMatchPattern = fileMatchPattern;
        }
    }
}
