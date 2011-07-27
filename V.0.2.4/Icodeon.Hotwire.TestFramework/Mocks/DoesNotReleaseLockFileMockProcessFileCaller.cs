using System;
using System.IO;
using Icodeon.Hotwire.Framework.Modules;
using Icodeon.Hotwire.Framework.Providers;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class DoesNotReleaseLockFileMockProcessFileCaller : ProcessFileCallerBase, IDisposable
    {
        private readonly string _stringToMatchInFileName;
        private readonly HotwireFilesProvider _filesProvider;
        private FileStream _locker = null;
        public FileStream Locker { get { return _locker; } }


        public DoesNotReleaseLockFileMockProcessFileCaller(string stringToMatchInFileName, HotwireFilesProvider filesProvider)
        {
            _stringToMatchInFileName = stringToMatchInFileName;
            _filesProvider = filesProvider;
        }

        public override void CallProcessFileWaitForComplete(string trackingNumber)
        {
            if (trackingNumber.Contains(_stringToMatchInFileName))
            {
                if (_locker != null) throw new Exception("locker already null, stringToMatchInFileName can only match a single file!");
                var fileToLock = Path.Combine(_filesProvider.ProcessingFolderPath, trackingNumber);
                _locker = new FileStream(fileToLock, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                _locker.Lock(0, 1);
                throw new Exception("some exception on second file, and BTW ... I've locked the file, mwaaa haaaa!");
            }
        }

        public void Dispose()
        {
            if (_locker != null) _locker.Dispose();
        }
    }


}
