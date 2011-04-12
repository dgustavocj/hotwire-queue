using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Icodeon.Hotwire.Framework.Contracts;

namespace Icodeon.Hotwire.Framework.Events
{
    public class FileInfoEventArgs : EventArgs
    {
        public FileInfo File { get; private set; }

        public FileInfoEventArgs(FileInfo file)
        {
            File = file;
        }
    }
}
