using System;
using System.Threading;
using System.Threading.Tasks;

namespace Icodeon.Hotwire.Framework.Threading
{
    public static class TaskExtensions
    {
        public static void ToNonscalingSync<T>(this Task<T> asyncCode, Action<T> code)
        {
            ToNonscalingSync(asyncCode,code, 0);
        }

        //Bloggable : write blog post, solicit feedback, ensure it's a workable solution for low scale.

        public static void ToNonscalingSync<T>(this Task<T> asyncCode, Action<T> code, int msTimeOut)
        {
            Exception asyncException = null;
            var waitForComplete = new AutoResetEvent(false);
            asyncCode.ContinueWith(t =>
                                       {
                                           try
                                           {
                                               code(t.Result);
                                           }
                                           catch (Exception ex)
                                           {
                                               asyncException = ex;
                                           }
                                           finally
                                           {
                                               waitForComplete.Set();
                                           }
                                       });
            if (msTimeOut!=0) 
                waitForComplete.WaitOne(msTimeOut);
            else
            {
                waitForComplete.WaitOne();
            }
            
            if (asyncException != null) throw (asyncException);
        }

    }
}