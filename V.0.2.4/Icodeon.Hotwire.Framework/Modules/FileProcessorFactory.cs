using System;
using System.IO;
using System.Linq;
using System.Threading;
using Icodeon.Hotwire.Contracts;
using Icodeon.Hotwire.Framework.Configuration;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Diagnostics;
using Icodeon.Hotwire.Framework.Providers;

namespace Icodeon.Hotwire.Framework.Modules
{
    public class FileProcessorFactory
    {

        private readonly IFileProcessorSection _config;
        private readonly IHotwireFileProcessorRelativeFolders _relativeFolders;
        private readonly LoggerBase _logger;
        private readonly IAppCounter _threadCounter;

        public FileProcessorFactory(IFileProcessorSection config, IHotwireFileProcessorRelativeFolders relativeFolders, LoggerBase logger, IAppCounter threadCounter)
        {
            _config = config;
            _relativeFolders = relativeFolders;
            _logger = logger;
            _threadCounter = threadCounter;
        }


        private ProcessFileRequestResult LogAndReturn(string reason, int numThreads, int numFiles)
        {
            _logger.Trace(reason);
            return new ProcessFileRequestResult { TotalThreads = numThreads, CntFilesWaitingToBeProcessed = numFiles };
        }

        public ProcessFileRequestResult ProcessFiles(string latestFile)
        {
            var fileProvider = new HotwireFilesProvider( _relativeFolders,_logger);     _logger.Trace("Threads={0}",_threadCounter.ReadCounter());
            int cntFilesToProcess = fileProvider.ProcessQueueFilePaths.Count();         _logger.Trace("processQueueFilePathsCount={0}", cntFilesToProcess);
            int threads = _threadCounter.ReadCounter();
            if (cntFilesToProcess == 0)
                return LogAndReturn("returning, nothing to process.", threads, 0);
            if (threads >= _config.MaxFileProcessorWorkers || (threads >= cntFilesToProcess))
                return LogAndReturn("returning, maximum number of file processors reached for number of files to process.", threads, cntFilesToProcess);
                
            // WARNING!
            // Its possible extra threads (more than configured) could be created, because we're not locking the FileProcessor Factory
            // risk of unforeseen complication caused by locked far exceeds risk of having an extra thread processing from time time.
            // only time this could be an issue is if the IFileProcessorProvider is not threadsafe!

            _logger.Trace("Starting new threads. Threads={0},MaxThreads={1}",_threadCounter.ReadCounter(),_config.MaxFileProcessorWorkers);

            _threadCounter.IncCounter(); _logger.Trace("Creating new thread.");
            Thread t = new Thread(() => ThreadProcessFiles(latestFile ?? "null")); _logger.Trace("starting thread running in the background.");
            t.Start(); _logger.Trace("finished starting thread, now returning to HttpModule");
            return new ProcessFileRequestResult {
                TotalThreads = threads+1,
                CntFilesWaitingToBeProcessed = fileProvider.ProcessQueueFilePaths.Count()
            };
        }

        
        public void ThreadProcessFiles(string filename)
        {
            try
            {
                _logger.Trace("\t\tThreadProcessFiles('{0}')", filename);
                _logger.Trace("Thread.CurrentPrincipal.Identity.Name:{0}", Thread.CurrentPrincipal.Identity.Name);
                _logger.Trace("\t\tThread -> background:{0},ThreadPoolThread:{1},ManagedThreadId:{2},Priority:{3}",Thread.CurrentThread.IsBackground,Thread.CurrentThread.IsThreadPoolThread,Thread.CurrentThread.ManagedThreadId,Thread.CurrentThread.Priority);
                _logger.Trace("\t\tCreating HotwireFilesProvider");

                var threadFileProvider = new HotwireFilesProvider(_relativeFolders, _logger);
                if (threadFileProvider.ProcessQueueFilePaths.Count() == 0)
                {
                    _logger.Trace("\t\tNo files to process, returning.");
                    return;
                }
                // the name of the thread is not the name of the file it is processing, it is the name of the first file that woke this thread up 
                _logger.Trace("\t\tsetting thread name");
                if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = string.Format("FirstFile:{0}", filename);
                ImportCartridgeDTO importCartridgeDto;
                _logger.Trace("\t\twhile more files left to process, move file to processing and start processing");
                while ((importCartridgeDto = threadFileProvider.GetBulkImportSettingsForNextFileToProcessAndMoveFileToProcessingOrNull()) != null)
                {
                    var file = importCartridgeDto.Cartridge;
                    try
                    {
                        string resource_file = Path.Combine(threadFileProvider.ProcessingFolderPath, file.ResourceFile);
                        _logger.Info("\t\t\tProcessing : " + importCartridgeDto.Cartridge.ResourceFile);
                        if (!File.Exists(resource_file)) throw new FileNotFoundException("Could not find resource file:" + resource_file);
                        _logger.Debug("\t\t\tCreating FileProcessor:{0},{1}", _config.AssemblyName, _config.TypeName);
                        var processor = Activator.CreateInstance(_config.AssemblyName, _config.TypeName).Unwrap() as IFileProcessorProvider;
                        if (processor==null)
                        {
                            throw new ApplicationException("Unable to create instance of FileProcessor, result was null.");
                        }
                        _logger.Debug("\t\t\tImportCartridge namevalue collection:{0}", importCartridgeDto.ToNameValueCollection().ToTraceString());
                        _logger.Debug("\t\t\tPROCESSING FILE -> processor.ProcessFile(resource_file='{0}', TransactionId='{1}', importCartridgeDto.ToNameValueCollection())",resource_file, file.TransactionId);
                        processor.ProcessFile(resource_file, file.TransactionId, importCartridgeDto.ToNameValueCollection());
                        // TODO: record processing finished!
                        threadFileProvider.MoveFileAndSettingsFileFromProcessingFolderToProcessedFolderLogSuccess(file.ResourceFile);
                    }
                    catch (Exception ex)
                    {
                        threadFileProvider.MoveFileAndSettingsFileFromProcessingFolderToErrorFolderWriteExceptionFile(file.ResourceFile, ex);
                        throw;
                    }
                } // while
                _logger.Trace("nextFile:null : (Thread {0} finishing.)", Thread.CurrentThread.Name);
                _logger.Trace("---------------");
            }
            // don't let the thread throw exception
            catch(Exception ex)
            {
                _logger.Trace("\t\t\tUnhandled fatal error during processing file thread:{0}",ex);
                _logger.FatalException("\t\t\tUnhandled fatal error during processing file thread:{0}",ex);
            }
            finally
            {
                _threadCounter.DecCounter();
            }
        }


    } // class
} // namespace
