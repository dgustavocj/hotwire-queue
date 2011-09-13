using System.Collections.Generic;
using System.IO;
using System.Linq;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.FolderWatcher
{
    public class FolderWatcher
    {
        private LoggerBase _logger;
        private readonly IConsoleWriter _writer;
        private bool _isMonitoring;

        // ADH: looks like _folderConfigs is never used, must check it out.
        private readonly List<FolderScript> _folderConfigs;
        private readonly Dictionary<string, FolderScript> _scripts;
        public Dictionary<string, FolderScript> Scripts
        {
            get { return _scripts;  }
        }

        public FolderWatcher(LoggerBase logger,IConsoleWriter writer, List<FolderConfig> folderConfigs)
        {
            _scripts = new Dictionary<string, FolderScript>();
            folderConfigs.ForEach( c=>
                                       {
                                           var folderScript = new FolderScript();
                                           folderScript.Config = c;
                                           var script = new ClassFactory(logger).CreateInstance<IFolderWatcherScript>(c.FolderWatcherScriptTypeNameCommaAssembly);
                                           var watcher = (folderScript.Watcher = new FileSystemWatcher(c.FolderPath, c.FileMatchPattern));
                                           watcher.EnableRaisingEvents = false;
                                           watcher.Created += FileCreated;
                                           folderScript.Script = script;
                                           _scripts.Add(c.Key, folderScript);
                                       });
            _isMonitoring = false;
            _logger = logger;
            _writer = writer;
        }

        public void StopMonitoring()
        {
            _scripts.ForEach((key, script) => script.Watcher.EnableRaisingEvents = false);
            _isMonitoring = false;
        }

        public void StartMonitoring()
        {
            if (_isMonitoring) return;
            _scripts.ForEach( (key,script) => script.Watcher.EnableRaisingEvents = true );
            _isMonitoring = true;
        }

        public FolderScript FindByFilePath(string filePath)
        {
            var fi = new FileInfo(filePath);
            var path = fi.Directory.FullName;
            var script = _scripts.FirstOrDefault(s => s.Value.Config.FolderPath.EqualsIgnoreCase(path));
            return script.Value;
        }

        private void FileCreated(object source, FileSystemEventArgs e)
        {
            var script = FindByFilePath(e.FullPath);
            if (script == null)
            {
                _logger.Warn("Tried to find script based on file Path of " + e.FullPath +". Script was null (no matching script found), cannot run any script, returning.");
                return;
            }
            if (script.Script.isRunning) return;
            // ADH 17.03.2011 folderPath is currently being ignored by the current implementation of IFolderWatcherScript 
            // because they're hard coded to use HotwireFilesProvider.
            script.Script.Run(_writer, script.Config.FolderPath);
        }

    }
}