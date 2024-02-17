using System;
using System.Collections.Generic;

namespace ThrowException.CSharpLibs.LogLib
{
    public interface ILogger : IDisposable
    {
        void Log(LogSeverity severity, string text, params object[] arguments);
    }

    public static class ILoggerExtensions
    {
        public static void Critical(this ILogger logger, Exception exception)
        {
            logger.Critical(exception.Message);
            logger.Debug(exception.StackTrace);
        }

        public static void Critical(this ILogger logger, string text, params object[] arguments)
        {
            logger.Log(LogSeverity.Critical, text, arguments);
        }

        public static void Error(this ILogger logger, string text, params object[] arguments)
        {
            logger.Log(LogSeverity.Error, text, arguments);
        }

        public static void Error(this ILogger logger, Exception exception)
        {
            logger.Error(exception.Message);
            logger.Debug(exception.StackTrace);
        }

        public static void Debug(this ILogger logger, string text, params object[] arguments)
        {
            logger.Log(LogSeverity.Debug, text, arguments);
        }

        public static void Info(this ILogger logger, string text, params object[] arguments)
        {
            logger.Log(LogSeverity.Info, text, arguments);
        }

        public static void Notice(this ILogger logger, string text, params object[] arguments)
        {
            logger.Log(LogSeverity.Notice, text, arguments);
        }

        public static void Verbose(this ILogger logger, string text, params object[] arguments)
        {
            logger.Log(LogSeverity.Verbose, text, arguments);
        }

        public static void Warning(this ILogger logger, string text, params object[] arguments)
        {
            logger.Log(LogSeverity.Warning, text, arguments);
        }
    }
}