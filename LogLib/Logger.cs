using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ThrowException.CSharpLibs.LogLib
{
    public class Logger : IDisposable, ILogger
    {
        public LogSeverity ConsoleSeverity { get; set; }

        public LogSeverity FileSeverity { get; private set; }

        private FileStream _logFile;

        private TextWriter _logWriter;

        private DateTime _logFileDate;

        public string LogFilePrefix { get; private set; }

        public List<ILogger> SubLoggers { get; private set; }

        public TimeSpan KeepLogFiles { get; set; } = TimeSpan.FromDays(5);

        public void EnableLogFile(LogSeverity severity, string logFilePrefix)
        {
            LogFilePrefix = logFilePrefix;
            FileSeverity = severity;
        }

        public Logger()
        {
            ConsoleSeverity = LogSeverity.Info;
            FileSeverity = LogSeverity.None;
            LogFilePrefix = null;
            SubLoggers = new List<ILogger>();
        }

        public void Dispose()
        {
            if (_logFile != null)
            {
                _logFile.Close();
                _logFile = null;
            }
        }

        private void CreateLogDir()
        {
            if (LogFilePrefix.EndsWith(Path.PathSeparator.ToString(), StringComparison.Ordinal))
            {
                CreateDir(LogFilePrefix.Substring(0, LogFilePrefix.Length - 1));
            }
            else
            {
                CreateDir(Path.GetDirectoryName(LogFilePrefix));
            }
        }

        private void CreateDir(string path)
        {
            if (path != Path.GetPathRoot(path))
            {
                CreateDir(Path.GetDirectoryName(path));
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private void RemoveOldLogFiles()
        {
            if (LogFilePrefix.EndsWith(Path.PathSeparator.ToString(), StringComparison.Ordinal))
            {
                RemoveOldLogFiles(new DirectoryInfo(LogFilePrefix.Substring(LogFilePrefix.Length - 1)), "*");
            }
            else
            {
                RemoveOldLogFiles(new DirectoryInfo(Path.GetDirectoryName(LogFilePrefix)), Path.GetFileName(LogFilePrefix));
            }
        }

        private void RemoveOldLogFiles(DirectoryInfo directory, string pattern)
        {
            foreach (var file in directory.GetFiles(pattern))
            { 
                if (file.LastWriteTime < DateTime.Now.Subtract(KeepLogFiles))
                {
                    file.Delete();
                }
            }
        }

        private void CheckLogFile()
        {
            if (_logFile == null)
            {
                CreateLogDir();
                _logFileDate = DateTime.Now;
				var filePath = string.Format("{0}_{1}.log", LogFilePrefix, _logFileDate.ToString("yyyy-MM-dd-HH-mm-ss"));
                _logFile = File.OpenWrite(filePath);
                _logWriter = new StreamWriter(_logFile);
            }
            else if (DateTime.Now > _logFileDate.AddDays(1))
            {
                _logFile.Close();
                _logFileDate = DateTime.Now;
                var filePath = string.Format("{0}_{1}.log", LogFilePrefix, _logFileDate.ToString("yyyy-MM-dd-HH-mm-ss"));
                _logFile = File.OpenWrite(filePath);
                _logWriter = new StreamWriter(_logFile);
            }
        }

        private IEnumerable<LogEntry> Write(LogSeverity severity, string text, params object[] arguments)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var fullText = arguments.Length > 0 ? string.Format(text, arguments) : text;

                foreach (var line in fullText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var trimmedLine = line.Trim();

                    if (trimmedLine != string.Empty)
                    {
                        yield return WriteLine(severity, trimmedLine);
                    }
                }

                foreach (var sub in SubLoggers)
                {
                    sub.Log(severity, text, arguments);
                }
            }
        }

        private LogEntry WriteLine(LogSeverity severity, string text)
        {
            var entry = new LogEntry(severity, text);

            if (entry.Severity >= ConsoleSeverity)
            {
                Console.Error.WriteLine(entry.ToText());
            }

            if (entry.Severity >= FileSeverity)
            {
                CheckLogFile();

                _logWriter.WriteLine(entry.ToText());
                _logWriter.Flush();
            }

            AdditionalWrite(entry);

            return entry;
        }

        public void Log(LogSeverity severity, string text, params object[] arguments)
        {
            Write(severity, text, arguments).ToList();
        }

        protected virtual void AdditionalWrite(LogEntry entry)
        { 
        }
    }
}
