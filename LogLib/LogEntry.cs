using System;
using System.Globalization;

namespace ThrowException.CSharpLibs.LogLib
{
    public class LogEntry
    {
        public LogSeverity Severity { get; private set; }

        public string Text { get; private set; }

        public DateTime DateTime { get; private set; }

        public LogEntry(LogSeverity severity, string text)
        {
            Severity = severity;
            Text = text;
            DateTime = DateTime.Now;
        }

        public string ToText()
        {
            return string.Format("{0} {1} {2}",
                                 DateTime.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture).PadRight(20),
                                 Severity.ToString().PadRight(8),
                                 Text);
        }
    }
}
