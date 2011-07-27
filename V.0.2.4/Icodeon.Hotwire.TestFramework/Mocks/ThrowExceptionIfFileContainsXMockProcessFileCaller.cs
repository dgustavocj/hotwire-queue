using System;
using Icodeon.Hotwire.Framework.Modules;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class ThrowExceptionIfFileContainsXMockProcessFileCaller : ProcessFileCallerBase
    {
        private readonly string _contains;
        private readonly string _messageInException;

        public ThrowExceptionIfFileContainsXMockProcessFileCaller(string contains, string messageInException)
        {
            _contains = contains;
            _messageInException = messageInException;
        }

        public override void CallProcessFileWaitForComplete(string trackingNumber)
        {
            if (trackingNumber.Contains(_contains))
            {
                throw new Exception(_messageInException);
            }
        }
    }
}
