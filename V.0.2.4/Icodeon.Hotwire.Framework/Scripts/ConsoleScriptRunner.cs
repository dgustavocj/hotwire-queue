using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Contracts;
using Icodeon.Hotwire.Framework.Providers;
using Icodeon.Hotwire.Framework.Utils;
using NLog;

namespace Icodeon.Hotwire.Framework.Scripts
{
    public class ConsoleScriptRunner
    {
        private readonly IConsoleWriter _console;
        private Logger _logger;

        public bool Abort { get; set; }

        private int _scriptNumber; 
        public int ScriptNumber
        {
            get { return _scriptNumber;  }
            set { _scriptNumber = value;  }
        }


        public void Pause()
        {
            _console.WriteLine("press enter to close.");
            _console.ReadLine();
        }

        public ConsoleScriptRunner(IConsoleWriter _console, string[] args, bool resolveEmbeddedAssemblies = false, Logger logger = null)
            : this(_console, resolveEmbeddedAssemblies, logger)
        {
            if (args.Length != 1 || !int.TryParse(args[0], out _scriptNumber)) 
            {
                var msg = "the only allowed argument is the number of which script you wish to run.";
                logger.Fatal(msg);
                _console.WriteLine(msg);
                Abort = true;
            }
        }

        public ConsoleScriptRunner(IConsoleWriter console, bool resolveEmbeddedAssemblies, Logger logger = null)
        {
            _console = console;
            if (resolveEmbeddedAssemblies) throw new ApplicationException("resolving embedded assemblies not currently supported.");
            Abort = false;
            _logger = logger ?? LogManager.GetCurrentClassLogger();
        }

        public bool RunConfiguration(Action config)
        {
            _console.WriteLine("configuring...");
            _logger.Trace("RunTheScript");
            try
            {
                config();
                return true;
            } 
            catch (ApplicationException appEx)
            {
                _logger.Error("Config exited due to an application exception.", appEx);
                // if it's an application exception, then I expect that the exception will already have been logged.
                // and appropriate UI message displayed.
                _logger.Info("Script exited with :false");
                _logger.Fatal(appEx.StackTrace);
                Pause();
                return false;
            }
            catch (Exception ex)
            {
                _logger.Fatal("Script exited due to an unhandled exception.");
                // any other exception is an unhandled exception and I assume no UI message has been displayed
                _logger.FatalException("Unhandled exception " + ex.Message, ex);
                _console.WriteLine("Fatal exception : " + ex.Message);
                _console.WriteLine(ex.StackTrace);
                _logger.Info("Script exited with :false");
                _logger.Fatal(ex.StackTrace);
                Pause();
                return false;
            }
        }


        public bool RunCode(Action<Logger> code,string scriptName, bool pauseBeforeReturning)
        {
            _console.WriteLine("Starting {0}", scriptName);
            _console.WriteLine("version:{0}, hotwire framework version:{1}", AssemblyHelper.ExecutingAssemblyVersion, AssemblyHelper.FrameworkVersion);
            _console.WriteLine("================================================================");
            _logger.Trace("RunTheScript");

            try
            {
                _logger.Info("Running script {0}.", scriptName);
                code(_logger);
                _logger.Info("Script exited with :true");
                if (pauseBeforeReturning) Pause();
                return true;
            } // try
            catch (ApplicationException appEx)
            {
                _logger.Error("Script exited due to an application exception.", appEx);
                // if it's an application exception, then I expect that the exception will already have been logged.
                // and appropriate UI message displayed.
                _logger.Info("Script exited with :false");
                _logger.Fatal(appEx.StackTrace);
                Pause();
                return false;
            }
            catch (Exception ex)
            {
                _logger.Fatal("Script exited due to an unhandled exception.");
                // any other exception is an unhandled exception and I assume no UI message has been displayed
                _logger.FatalException("Unhandled exception " + ex.Message, ex);
                _console.WriteLine("Fatal exception : " + ex.Message);
                _console.WriteLine(ex.StackTrace);
                _logger.Info("Script exited with :false");
                _logger.Fatal(ex.StackTrace);
                Pause();
                return false;
            }
            
        }

        public bool RunTheScript(IScript script, string folderPath, bool pauseBeforeReturning)
        {
            if (script==null)
            {
                _console.WriteLine("Aborting starting {0}, configuration did not complete.", script.ScriptName);
                return false;
            }
            _console.WriteLine("Starting {0}", script.ScriptName);
            _console.WriteLine("version:{0}, hotwire framework version:{1}", AssemblyHelper.ExecutingAssemblyVersion, AssemblyHelper.FrameworkVersion);
            _console.WriteLine("================================================================");
             // resolve embedded assemblies so that we can deploy a single exe instead of multiple files.
            _logger.Trace("RunTheScript");
            // dont run the script because could not parse the arguments!
            if (Abort) return false;
            
            try
            {
                _logger.Info("Running script {0}.",script.ScriptName);
                script.Run(_logger, _console);
                _logger.Info("Script exited with :true");
                if(pauseBeforeReturning) Pause();
                return true;
            } // try
            catch (ApplicationException appEx)
            {
                _logger.Error("Script exited due to an application exception.", appEx);
                // if it's an application exception, then I expect that the exception will already have been logged.
                // and appropriate UI message displayed.
                _logger.Info("Script exited with :false");
                _logger.Fatal(appEx.StackTrace);
                Pause();
                return false;
            }
            catch (Exception ex)
            {
                _logger.Fatal("Script exited due to an unhandled exception.");
                // any other exception is an unhandled exception and I assume no UI message has been displayed
                _logger.FatalException("Unhandled exception " + ex.Message, ex);
                _console.WriteLine("Fatal exception : " + ex.Message);
                _console.WriteLine(ex.StackTrace);
                _logger.Info("Script exited with :false");
                _logger.Fatal(ex.StackTrace);
                Pause();
                return false;
            }
        }


    }
}
