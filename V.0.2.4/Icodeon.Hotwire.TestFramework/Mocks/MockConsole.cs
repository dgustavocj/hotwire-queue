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
        private readonly bool _echoReadLines;

        public class Line
        {
            public Line(Action beforeReadLine, string text)
            {
                Text = text;
                BeforeReadLine = beforeReadLine;
            }

            public string Text { get; set; }
            public Action BeforeReadLine { get; set; }
        }

        private Queue<Line> _lines;

        private MockConsole() { }

        public MockConsole(List<Line> lines, IDateTime dateTime, bool echoReadLines) : base(dateTime)
        {
            _echoReadLines = echoReadLines;
            _lines = new Queue<Line>(lines.Count);
          lines.ForEach(_lines.Enqueue);  
        }

        public override string ReadLine()
        {
            var line = _lines.Dequeue();
            var temp = line.BeforeReadLine; if (temp != null) temp(); 
            if (_echoReadLines) Console.WriteLine(line.Text);
            return line.Text;
        }
    }
}
