using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.LogLib
{
    public class DummyLogger : ILogger
    {
        public void Dispose()
        {
        }

        public void Log(LogSeverity severity, string text, params object[] arguments)
        {
        }
    }
}
