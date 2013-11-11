using System;

namespace SimpleLogging.Core
{
    public interface ILoggingService
    {
        /// <summary>
        ///     Every logger should have a name :)
        /// </summary>
        string Name { get; }

        void Debug(string message);
        void Debug(string message, params object[] args);

        void Info(string message);
        void Info(string message, params object[] args);

        void Warn(string message);
        void Warn(string message, params object[] args);

        void Error(string message);
        void Error(string message, params object[] args);
        void Error(Exception exception, string message = null);

        void Fatal(string message);
        void Fatal(string message, params object[] args);
        void Fatal(Exception exception, string message = null);
    }
}