using System;

namespace SimpleLogging.Core
{
    public interface ILoggingService
    {
        /// <summary>
        ///     Every logger should have a name :)
        /// </summary>
        /// <remarks>If this is null AND you haven't set it, then this is auto determined via the stackFrame recusing up until it finds a non SimpleLogging method and not a System method/property.</remarks>
        string Name { get; }

        void Trace(string message);
        void Trace(string message, params object[] args);

        void Debug(string message);
        void Debug(string message, params object[] args);

        void Info(string message);
        void Info(string message, params object[] args);

        void Warning(string message);
        void Warning(string message, params object[] args);

        void Error(string message);
        void Error(string message, params object[] args);
        void Error(Exception exception, string message = null, bool isStackTraceIncluded = true);

        void Fatal(string message);
        void Fatal(string message, params object[] args);
        void Fatal(Exception exception, string message = null, bool isStackTraceIncluded = true);
    }
}