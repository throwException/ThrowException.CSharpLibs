using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.LogLib
{
    public class LogTee : ILogger
    {
        private readonly List<ILogger> _loggers;

        public void Add(ILogger logger)
        {
            _loggers.Add(logger);
        }

        public void Add(IEnumerable<ILogger> loggers)
        {
            _loggers.AddRange(loggers);
        }

        public LogTee(params ILogger[] loggers)
        {
            _loggers = new List<ILogger>(loggers);
        }

        public void Log(LogSeverity severity, string text, params object[] arguments)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(severity, text, arguments);
            }
        }

        public void Dispose()
        {
            foreach (var logger in _loggers)
            {
                logger.Dispose();
            }
            _loggers.Clear();
        }

        public void Flush()
        {
            foreach (var logger in _loggers)
            {
                logger.Flush();
            }
        }

        public void Process()
        {
            foreach (var logger in _loggers)
            {
                logger.Process();
            }
        }
    }
}
