using System;

namespace ThrowException.CSharpLibs.LogLib
{
    public interface ILogger
    {
        void Critical(Exception exception);
        void Critical(string text, params object[] arguments);
        void Error(string text, params object[] arguments);
        void Error(Exception exception);
        void Debug(string text, params object[] arguments);
        void Info(string text, params object[] arguments);
        void Notice(string text, params object[] arguments);
        void Verbose(string text, params object[] arguments);
        void Warning(string text, params object[] arguments);
    }
}