using System;
using Icodeon.Hotwire.Framework.Modules;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class MockProcessFileCaller : ProcessFileCallerBase
    {
        private readonly Action<string> _mockCallProcess;

        public MockProcessFileCaller() : this(null) {}

        public MockProcessFileCaller(Action<string> mockCallProcess)
        {
            _mockCallProcess = mockCallProcess;
        }

        public override void CallProcessFileWaitForComplete(string trackingNumber)
        {
            if (_mockCallProcess != null) _mockCallProcess(trackingNumber);
        }
    }
}
