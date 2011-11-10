using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Icodeon.Hotwire.Framework.Utils;

namespace Icodeon.Hotwire.TestFramework.Mocks
{
    public class MockConsole : ConsoleWriter
    {
        public class Line
        {
            public Line(string text, Action beforeReadLine)
            {
                Text = text;
                BeforeReadLine = beforeReadLine;
            }

            public string Text { get; set; }
            public Action BeforeReadLine { get; set; }
        }

        private Stack<Line> _lines;

        private MockConsole() { }

        public MockConsole(List<Line> lines, IDateTime dateTime) : base(dateTime)
        {
            _lines = new Stack<Line>(lines.Count);
          lines.ForEach(_lines.Push);  
        }

        public override string ReadLine()
        {
            var line = _lines.Pop();
            var temp = line.BeforeReadLine; if (temp != null) temp(); 
            return line.Text;
        }
    }
}
