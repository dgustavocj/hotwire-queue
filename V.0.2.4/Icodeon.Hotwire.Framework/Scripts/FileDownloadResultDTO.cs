using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Icodeon.Hotwire.Framework.Scripts
{
    public class FileDownloadResultDTO
    {
        public double Kilobytes { get; set; }
        public double Seconds { get; set; }
        public double KbPerSec { get; set; }
        public FileInfo DownloadedFile { get; set; }
    }
}
