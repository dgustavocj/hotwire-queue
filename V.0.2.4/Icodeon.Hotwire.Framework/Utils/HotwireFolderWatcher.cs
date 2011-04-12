using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Providers;

namespace Icodeon.Hotwire.Framework.Utils
{
    public class HotwireFolderWatcher
    {
        private readonly HotwireFilesProvider _fileProvider;
        private readonly Action _onChanged;

        private FileSystemWatcher downloadingWatcher;
        private FileSystemWatcher processQueueWatcher;
        private FileSystemWatcher processingWatcher;
        private FileSystemWatcher processedQueueWatcher;

        public HotwireFolderWatcher(HotwireFilesProvider fileProvider, Action onChanged)
        {
            _fileProvider = fileProvider;
            _onChanged = onChanged;
            downloadingWatcher = new FileSystemWatcher();
            downloadingWatcher.Path = fileProvider.DownloadingFolderPath;
            downloadingWatcher.Created += new FileSystemEventHandler(OnChanged);
            downloadingWatcher.Deleted += new FileSystemEventHandler(OnChanged);
            downloadingWatcher.EnableRaisingEvents = true;

            processQueueWatcher = new FileSystemWatcher();
            processQueueWatcher.Path = fileProvider.ProcessQueueFolderPath;
            processQueueWatcher.Created += new FileSystemEventHandler(OnChanged);
            processQueueWatcher.Deleted += new FileSystemEventHandler(OnChanged);
            processQueueWatcher.EnableRaisingEvents = true;

            processingWatcher = new FileSystemWatcher();
            processingWatcher.Path = fileProvider.ProcessingFolderPath;
            processingWatcher.Created += new FileSystemEventHandler(OnChanged);
            processingWatcher.Deleted += new FileSystemEventHandler(OnChanged);
            processingWatcher.EnableRaisingEvents = true;

            processedQueueWatcher = new FileSystemWatcher();
            processedQueueWatcher.Path = fileProvider.ProcessedFolderPath;
            processedQueueWatcher.Created += new FileSystemEventHandler(OnChanged);
            processedQueueWatcher.Deleted += new FileSystemEventHandler(OnChanged);
            processedQueueWatcher.EnableRaisingEvents = true;
        }


        private void OnChanged(object source, FileSystemEventArgs e)
        {
            // keeping it very simple for now, if anything changes, then we call Action, nothing else.
            Action temp = _onChanged;
            if (temp != null) temp();
        }

    }
}
