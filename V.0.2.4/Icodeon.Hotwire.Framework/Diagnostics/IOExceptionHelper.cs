using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Diagnostics
{
    public static class IOExceptionHelper
    {
        public static bool FileAlreadyExists(this IOException ioex)
        {
            return ioex.Message.Contains("already exists");
        }
    }
}
