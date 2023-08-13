using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.LogLib
{
    public class DummyLogger : ILogger
    {
        public List<ILogger> SubLoggers { get; private set; } = new List<ILogger>();

        public void Critical(Exception exception)
        {
        }

        public void Critical(string text, params object[] arguments)
        {
        }

        public void Debug(string text, params object[] arguments)
        {
        }

        public void Error(string text, params object[] arguments)
        {
        }

        public void Error(Exception exception)
        {
        }

        public void Info(string text, params object[] arguments)
        {
        }

        public void Log(LogSeverity severity, string text, params object[] arguments)
        {
        }

        public void Notice(string text, params object[] arguments)
        {
        }

        public void Verbose(string text, params object[] arguments)
        {
        }

        public void Warning(string text, params object[] arguments)
        {
        }
    }
}
