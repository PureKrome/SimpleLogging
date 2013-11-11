using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using SimpleLogging.Core;

namespace SimpleLogging.NLog
{
    public class NLogLoggingService : ILoggingService
    {
        private ReadOnlyCollection<Target> _allTargets;
        private Logger _log;
        private IList<LoggingRule> _loggingRules;

        /// <summary>
        ///     Add an NLogViewerTarget for the defined log level.
        /// </summary>
        /// <param name="name">Name of the logger.</param>
        public NLogLoggingService(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name",
                    "A logger 'name' is required so it's possible to filter the log results.");
            }

            Name = name;

            RememberExistingFileConfiguration();
        }

        /// <summary>
        ///     Add an NLogViewerTarget for the defined log level.
        /// </summary>
        /// <param name="name">Name of the logger.</param>
        /// <param name="address">
        ///     Network address where the messages will be sent to. eg. udp://1.2.3.4:9999). Refer to NLog docs
        ///     for all the  address options.
        /// </param>
        /// <param name="minimumLogLevel">
        ///     Optional: Trace, Debug, Info, Warning, Error, Fatal, Off. If you make a type, then it
        ///     defaults to Off.
        /// </param>
        public NLogLoggingService(string name, string address, string minimumLogLevel = "debug") : this(name)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentNullException("address");
            }

            if (string.IsNullOrWhiteSpace(minimumLogLevel))
            {
                throw new ArgumentNullException("minimumLogLevel");
            }

            ConfigureNLogViewerTarget(address, minimumLogLevel);
        }

        /// <summary>
        ///     Add an NLogViewerTarget for the defined log level.
        /// </summary>
        /// <param name="name">Name of the logger.</param>
        /// <param name="target">Custom configured NLogViewerTarget.</param>
        /// <param name="minimumLogLevel">
        ///     Optional: Trace, Debug, Info, Warning, Error, Fatal, Off. If you make a type, then it
        ///     defaults to Off.
        /// </param>
        public NLogLoggingService(string name, NLogViewerTarget target, string minimumLogLevel = "debug")
            : this(name)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (string.IsNullOrWhiteSpace(minimumLogLevel))
            {
                throw new ArgumentNullException("minimumLogLevel");
            }

            ConfigureNLogViewerTarget(target, minimumLogLevel);
        }

        protected Logger Logger
        {
            get
            {
                // Lazy loading - no need to sync lock, cause this is only for this object-instance .. not the entire application.
                return _log ??
                       (_log =
                           (string.IsNullOrEmpty(Name)
                               ? LogManager.GetCurrentClassLogger()
                               : LogManager.GetLogger(Name)));
            }
        }

        #region Implementation of ILoggingService

        public string Name { get; private set; }

        public void Debug(string message)
        {
            if (Logger != null && Logger.IsDebugEnabled)
            {
                Logger.Debug(message);
            }
        }

        public void Debug(string message, params object[] args)
        {
            if (Logger != null && Logger.IsDebugEnabled)
            {
                Logger.Debug(message, args);
            }
        }

        public void Info(string message)
        {
            if (Logger != null && Logger.IsInfoEnabled)
            {
                Logger.Info(message);
            }
        }

        public void Info(string message, params object[] args)
        {
            if (Logger != null && Logger.IsInfoEnabled)
            {
                Logger.Info(message, args);
            }
        }

        public void Warning(string message)
        {
            if (Logger != null && Logger.IsWarnEnabled)
            {
                Logger.Warn(message);
            }
        }

        public void Warning(string message, params object[] args)
        {
            if (Logger != null && Logger.IsWarnEnabled)
            {
                Logger.Warn(message, args);
            }
        }

        public void Error(string message)
        {
            if (Logger != null && Logger.IsErrorEnabled)
            {
                Logger.Error(message);
            }
        }

        public void Error(string message, params object[] args)
        {
            if (Logger != null && Logger.IsErrorEnabled)
            {
                Logger.Error(message, args);
            }
        }

        public void Error(Exception exception, string message = null)
        {
            if (Logger == null || !Logger.IsErrorEnabled)
            {
                return;
            }

            string errorMessage = string.Format("{0}{1}{2}",
                string.IsNullOrWhiteSpace(message) ? string.Empty : message,
                string.IsNullOrWhiteSpace(message) ? string.Empty : " Exception Message: ",
                exception.Message);

            Error(errorMessage);
        }

        public void Fatal(string message)
        {
            if (Logger != null && Logger.IsFatalEnabled)
            {
                Logger.Fatal(message);
            }
        }

        public void Fatal(string message, params object[] args)
        {
            if (Logger != null && Logger.IsFatalEnabled)
            {
                Logger.Fatal(message, args);
            }
        }

        public void Fatal(Exception exception, string message = null)
        {
            string errorMessage = string.Format("{0}{1}{2}",
                string.IsNullOrWhiteSpace(message) ? string.Empty : message,
                string.IsNullOrWhiteSpace(message) ? string.Empty : " Exception Message: ",
                exception.Message);

            Fatal(errorMessage);
        }

        #endregion

        /// <summary>
        ///     Adds or updates an NLog Viewer Target.
        /// </summary>
        /// <remarks>
        ///     This is a nice, clean and programmable way to send logging messages via the NLog Viewer Target.
        ///     Messages can be sent via Udp, Tcp or Http.
        ///     A good use of this is in Azure to programatically update a role without requiring it to recyle the entire role.
        ///     NOTE: The NLogViewerTarget will be wrapped in an AsyncWrapperTarget. Refer to the offical docs, if you don't know
        ///     what that means.
        /// </remarks>
        /// <param name="address">
        ///     Network address where the messages will be sent to. eg. udp://1.2.3.4:9999). Refer to NLog docs
        ///     for all the  address options.
        /// </param>
        /// <param name="minimumLogLevel">
        ///     Optional: Trace, Debug, Info, Warning, Error, Fatal, Off. If you make a type, then it
        ///     defaults to Off.
        /// </param>
        /// <see cref="https://github.com/nlog/NLog/wiki/NLogViewer-target" />
        public void ConfigureNLogViewerTarget(string address, string minimumLogLevel = "debug")
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentNullException("address");
            }

            if (string.IsNullOrWhiteSpace(minimumLogLevel))
            {
                throw new ArgumentNullException("minimumLogLevel");
            }

            var target = new NLogViewerTarget
            {
                Name = "nLogViewerTarget-" + Guid.NewGuid(),
                Address = address,
                IncludeNLogData = false
            };

            ConfigureNLogViewerTarget(target, minimumLogLevel);
        }

        /// <summary>
        ///     Adds or updates an NLog Viewer Target.
        /// </summary>
        /// <remarks>
        ///     This is a nice, clean and programmable way to send logging messages via the NLog Viewer Target.
        ///     Messages can be sent via Udp, Tcp or Http.
        ///     A good use of this is in Azure to programatically update a role without requiring it to recyle the entire role.
        ///     NOTE: The NLogViewerTarget will be wrapped in an AsyncWrapperTarget. Refer to the offical docs, if you don't know
        ///     what that means.
        /// </remarks>
        /// <param name="target">Custom configured NLogViewerTarget.</param>
        /// <param name="minimumLogLevel">
        ///     Optional: Trace, Debug, Info, Warning, Error, Fatal, Off. If you make a type, then it
        ///     defaults to Off.
        /// </param>
        /// <see cref="https://github.com/nlog/NLog/wiki/NLogViewer-target" />
        public void ConfigureNLogViewerTarget(NLogViewerTarget target, string minimumLogLevel = "debug")
        {
            // Code Reference: http://stackoverflow.com/questions/7471490/add-enable-and-disable-nlog-loggers-programmatically</remarks>

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (string.IsNullOrWhiteSpace(minimumLogLevel))
            {
                throw new ArgumentNullException("minimumLogLevel");
            }

            LogManager.Configuration = new LoggingConfiguration();

            // Make sure we don't foget about our 'default' values.
            AddExistingTargetsAndRules();


            // We need to make sure we wrap the target in an Async Target Wrapper.
            var asyncWrapperTarget = new AsyncTargetWrapper(target);
            LogManager.Configuration.AddTarget("asyncWrapperTarget-" + Guid.NewGuid(), asyncWrapperTarget);


            LogLevelType logLevel;
            Enum.TryParse(minimumLogLevel, true, out logLevel);

            var loggingRule = new LoggingRule("*", TryParseToLogLevel(logLevel), asyncWrapperTarget);
            LogManager.Configuration.LoggingRules.Add(loggingRule);

            LogManager.ReconfigExistingLoggers();
        }

        /// <summary>
        ///     This remembers any confuration data defined in a nlog.config file.
        /// </summary>
        private void RememberExistingFileConfiguration()
        {
            if (LogManager.Configuration == null)
            {
                return;
            }

            if (LogManager.Configuration.AllTargets != null)
            {
                _allTargets = LogManager.Configuration.AllTargets;
            }

            if (LogManager.Configuration.LoggingRules != null)
            {
                _loggingRules = LogManager.Configuration.LoggingRules;
            }
        }

        private void AddExistingTargetsAndRules()
        {
            if (_allTargets == null)
            {
                return;
            }

            foreach (Target target in _allTargets)
            {
                LogManager.Configuration.AddTarget(target.Name, target);
            }

            if (_loggingRules != null)
            {
                foreach (LoggingRule loggingRule in _loggingRules)
                {
                    LogManager.Configuration.LoggingRules.Add(loggingRule);
                }
            }
        }

        private static LogLevel TryParseToLogLevel(LogLevelType logLevelType)
        {
            switch (logLevelType)
            {
                case LogLevelType.Trace:
                    return LogLevel.Trace;
                case LogLevelType.Debug:
                    return LogLevel.Debug;
                case LogLevelType.Info:
                    return LogLevel.Info;
                case LogLevelType.Warning:
                    return LogLevel.Warn;
                case LogLevelType.Error:
                    return LogLevel.Error;
                case LogLevelType.Fatal:
                    return LogLevel.Fatal;
                case LogLevelType.Off:
                case LogLevelType.Unknown:
                    return LogLevel.Off;
                default:
                    throw new ArgumentOutOfRangeException("logLevelType");
            }
        }
    }
}