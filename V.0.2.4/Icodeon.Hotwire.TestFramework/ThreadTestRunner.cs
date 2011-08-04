using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace Icodeon.Hotwire.TestFramework
{
    //TODO: write unit test
    public class ThreadTestRunner
    {
        public List<Exception> Exceptions { get; set; }
        public Exception LastException { get; set; }


        private readonly int _msThreadTimeout;
        private List<Thread> _threads;

        public ThreadTestRunner() : this( int.MaxValue)
        {
            
        }

        public ThreadTestRunner(int msThreadTimeout)
        {
            _msThreadTimeout = msThreadTimeout;
            Exceptions = new List<Exception>();
            _threads = new List<Thread>();
        }

        public void RunInParallel(Action[] actions)
        {
            actions.ToList().ForEach(
                a=>
                    {
                        var thread = new Thread(()=> RunAction(a));
                        _threads.Add(thread);
                        thread.Start();
                    }
                );
            // wait for all threads to complete
            _threads.ForEach(t=> t.Join(_msThreadTimeout));
            if (LastException != null) throw new ThreadTestRunnerException(LastException, Exceptions);
        }



        private void RunAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Exceptions.Add(ex);
                LastException = ex;
            }
            
        }

    }
}
