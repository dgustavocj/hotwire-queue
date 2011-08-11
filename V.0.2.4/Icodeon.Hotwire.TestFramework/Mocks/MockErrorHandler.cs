using System;
using Icodeon.Hotwire.Framework.Diagnostics;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class MockErrorHandler : IExceptionHandler
    {
        public bool Handled { get; set; }
        public ExceptionEventArgs ExceptionArgs { get; set; }

        public void HandleException(object sender, ExceptionEventArgs args)
        {
            Handled = true;
            ExceptionArgs = args;
        }
    }
}