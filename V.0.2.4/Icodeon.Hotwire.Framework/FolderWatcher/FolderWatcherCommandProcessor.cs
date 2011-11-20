using System;
using System.IO;
using System.Linq;
using System.Threading;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Http;
using Icodeon.Hotwire.Framework.Modules;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Scripts;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.FolderWatcher
{
    public static class FolderWatcherCommandProcessor
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private static bool isDownloading = false;
        private static bool isProcessing = false;

        public static void DeleteAllRunningFiles()
        {
            FolderWatcherCommandProcessor.DeleteDownloadRunningFile();
            FolderWatcherCommandProcessor.DeleteProcessingRunningFile();
        }

        public static void CheckFoldersExistCreateIfNeeded(IConsoleWriter console)
        {
            HotwireFilesProvider.CreateFoldersIfNotExist(f => console.WriteLine("Creating folder:{0}", f));
        }

        public static void DeleteAnyPreExistingRunningFiles(HotwireFilesProvider filesProvider)
        {
            CreateDownloadRunningFile(filesProvider);
            CreateProcessingRunningFile(filesProvider);
            DeleteAllRunningFiles();
        }


        private const string DownloadRunningFile = "Download.running";
        private const string ProcessRunningFile = "Process.running";

        private static string _downloadRunningFile;
        private static string _processRunningFile;



        private const string processor = "process";
        private const string download = "download";



        private static void CreateDownloadRunningFile(HotwireFilesProvider filesProvider)
        {
            _downloadRunningFile = Path.Combine(filesProvider.DownloadQueueFolderPath, DownloadRunningFile);
            if (!File.Exists(_downloadRunningFile)) File.WriteAllText(_downloadRunningFile, "downloading");
        }

        private static void CreateProcessingRunningFile(HotwireFilesProvider filesProvider)
        {
            _processRunningFile = Path.Combine(filesProvider.ProcessQueueFolderPath, ProcessRunningFile);
            if (!File.Exists(_processRunningFile)) File.WriteAllText(_processRunningFile, "processing");
        }

        private static void DeleteProcessingRunningFile()
        {
            if (File.Exists(_processRunningFile)) File.Delete(_processRunningFile);
        }

        private static void DeleteDownloadRunningFile()
        {
            if (File.Exists(_downloadRunningFile)) File.Delete(_downloadRunningFile);
        }

        private static ProcessFileCallerBase _processFileCaller = null;
        private static IClientDownloader _clientDownloader = null;


        public static void RunDownloader(string fileName, IConsoleWriter console, HotwireFilesProvider filesProvider, IDateTime dateTime, IClientDownloader clientDownloader)
        {
            if (isDownloading) return;
            try
            {
                isDownloading = true;
                console.Write("\n");
                console.Log("Starting downloader.");
                string msg = "new file '" + fileName + "' in download queue.";
                _logger.Debug(msg);
                //var clientDownloader = new HotClient();
                var downloadScript = new FileDownloaderScript(filesProvider, clientDownloader, dateTime);
                downloadScript.Run(console);
            }
            finally
            {
                _logger.Debug("\t...finished downloading.");
                isDownloading = false;
            }

        }

        private static void DownloadQueueFileCreated(object source, FileSystemEventArgs e)
        {
            _logger.Debug("DownloadQueueFileCreated");
            if (!isDownloading)
            {
                _logger.Debug("not already downloading, starting new thread.");
                Thread thread = new Thread(() => RunDownloader(e.Name));
                thread.Start();
            }
        }

        private static void RunDownloader(string fileName)
        {
            var console = new ConsoleWriter();
            var filesProvider = HotwireFilesProvider.GetFilesProviderInstance();
            RunDownloader(fileName, console, filesProvider, new DateTimeWrapper(),_clientDownloader);
        }

        private static void ProcessQueueFileCreated(object source, FileSystemEventArgs e)
        {
            if (!isProcessing)
            {
                // don't start a new thread/file processor until the file has finished downloading

                if (!e.FullPath.Equals(processor) && new FileInfo(e.FullPath).Length == 0) return;
                Thread thread = new Thread(() => RunProcessor(e.Name));
                thread.Start();
            }
        }

        public static void RunProcessor(string fileName, IConsoleWriter console, HotwireFilesProvider filesProvider, ProcessFileCallerBase processFileCaller)
        {
            if (isProcessing) return;
            try
            {
                isProcessing = true;
                string msg = "new file " + fileName + " in process queue. Starting processor.";
                _logger.Trace(msg);
                console.Log("Starting processor.");
                var processorScript = new FileProcessorScript(filesProvider, processFileCaller);
                processorScript.Run(console);
            }
            finally
            {
                isProcessing = false;
            }
        }


        public static void RunProcessor(string fileName)
        {
            var console = new ConsoleWriter();
            var filesProvider = HotwireFilesProvider.GetFilesProviderInstance();
            RunProcessor(fileName, console, filesProvider, _processFileCaller);
        }


        public static void RunCommandProcessorUntilExit(bool autostart, HotwireFilesProvider filesProvider, IConsoleWriter console, IDateTime dateTime, ProcessFileCallerBase processFileCaller, IClientDownloader clientDownloader)
        {
            _clientDownloader = clientDownloader;
            _processFileCaller = processFileCaller;
            var processorWatcher = new FileSystemWatcher(filesProvider.ProcessQueueFolderPath);
            processorWatcher.Created += ProcessQueueFileCreated;
            processorWatcher.EnableRaisingEvents = false;
            processorWatcher.IncludeSubdirectories = false;
            // http://msdn.microsoft.com/en-us/library/system.io.filesystemwatcher.internalbuffersize.aspx
            processorWatcher.InternalBufferSize = 64000;

            FileSystemWatcher downloadWatcher = new FileSystemWatcher(filesProvider.DownloadQueueFolderPath);
            downloadWatcher.Created += DownloadQueueFileCreated;
            downloadWatcher.EnableRaisingEvents = false;
            downloadWatcher.IncludeSubdirectories = false;
            processorWatcher.InternalBufferSize = 64000;

            console.WriteLine("Enter 'help' for list of commands.\n");
            var command = new CommandArgument();
            while (!(command.Cmd.Equals("exit", StringComparison.OrdinalIgnoreCase)))
            {
                console.Write(">");
                if (autostart)
                {
                    console.WriteBold("autostarting...\n");
                    command = new CommandArgument { Cmd = "start", Argument = null };
                    autostart = false;
                }
                else
                {
                    command = console.ReadLine().ParseCommand(console);
                }

                _logger.Info("recieved command:{0}", command);
                try
                {
                    switch (command.Cmd)
                    {
                        case "test-data":
                            command.AssertArgumentRequired();
                            filesProvider.RefreshFiles();
                            var files = filesProvider.TestDataFilePaths.Where(f => f.Contains(command.Argument)).ToList();
                            if (files.Count() == 0)
                                console.Log("No *.import files found in " + filesProvider.TestDataFolderPath + "\n matching your filter.");
                            else
                                files.ForEach(f =>
                                                  {
                                                      var fi = new FileInfo(f);
                                                      fi.CopyTo(Path.Combine(filesProvider.ProcessQueueFolderPath, fi.Name));
                                                      console.WriteLine(fi.Name);
                                                  });
                            console.WriteLine("...queued for processing.");
                            break;

                        case "test-error": throw new ApplicationException("test-error: test message to confirm logging is working.");
                            break;

                        case download:
                            CreateDownloadRunningFile(filesProvider);
                            DeleteProcessingRunningFile();
                            downloadWatcher.EnableRaisingEvents = true;
                            console.LogBold("Stopping process queue monitoring.");
                            processorWatcher.EnableRaisingEvents = false;    
                            Thread downloadThread = new Thread(() => RunDownloader(download));
                            console.Log("Now monitoring download queue '{0}'.", Path.GetFileName(filesProvider.DownloadQueueFolderPath));
                            downloadThread.Start();
                            break;

                        case processor:
                            CreateProcessingRunningFile(filesProvider);
                            DeleteDownloadRunningFile();
                            console.LogBold("Stopping download queue monitoring.");
                            downloadWatcher.EnableRaisingEvents = false;    
                            processorWatcher.EnableRaisingEvents = true;
                            Thread processorThread = new Thread(() => RunProcessor(processor));
                            processorThread.Start();
                            console.Log("Now monitoring process queue '{0}'.",Path.GetFileName(filesProvider.ProcessQueueFolderPath));
                            break;


                        case "cls":
                            console.Clear();
                            break;

                        case "stop":
                            DeleteDownloadRunningFile();
                            DeleteProcessingRunningFile();
                            downloadWatcher.EnableRaisingEvents = false;
                            processorWatcher.EnableRaisingEvents = false;
                            console.Log("All folder monitoring stopped.");
                            break;

                        case "start":
                            CheckFoldersExistCreateIfNeeded(console);
                            CreateDownloadRunningFile(filesProvider);
                            CreateProcessingRunningFile(filesProvider);
                            downloadWatcher.EnableRaisingEvents = true;
                            processorWatcher.EnableRaisingEvents = true;
                            console.Log("All folder monitoring resumed.");
                            break;


                        case "help":
                            console.WriteLine("FolderWatcher commands:");
                            console.WriteLine("-----------------------");
                            console.WriteLine("exit\t\tStop monitoring folders and exit.");
                            console.WriteLine("help\t\tDisplay list of commands.");
                            console.WriteLine("process\t\tForce processing any files in the process queue.");
                            console.WriteLine("process [fileFilterString]\n\t\tForce processing a single file containing filterString.");
                            console.WriteLine("download\tForce a download of any files in the download queue.");
                            console.WriteLine("cls\t\tClear the screen.");
                            console.WriteLine("test-error\tWrite test message to error logfile.");
                            console.WriteLine("test-data [fileFilterString] \tloads * from TestFiles into ProcessQueue.");
                            console.WriteLine("stop\t\tStop all monitoring.");
                            console.WriteLine("start\t\tResume all monitoring.");
                            console.WriteLine("");
                            break;

                        case "exit":
                            DeleteDownloadRunningFile();
                            DeleteProcessingRunningFile();
                            console.WriteLine("Goodbye!");
                            break;

                        default:
                            console.WriteLine("'{0}' is not a recognised command.", command.Cmd);
                            break;
                    }

                }
                catch (Exception ex)
                {
                    DeleteDownloadRunningFile();
                    DeleteProcessingRunningFile();
                    console.WriteLine(ex.Message);
                    _logger.Error(ex.ToString());
                }

            }
        }



    }
}
