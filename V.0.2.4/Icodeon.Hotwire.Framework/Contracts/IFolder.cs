using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Contracts
{
    public interface IFolder
    {
        bool Exists(string file);

        // move a file from folder A to folder B
        void MoveTo(string source, IFolder destination);
        string Path { get;  }
    }
}
