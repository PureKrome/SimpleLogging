﻿using System;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using SimpleLogging.Core;

namespace SimpleLogging.NLog
{
    public class NLogLoggingService : ILoggingService
    {
        private Logger _log;

        /// <summary>
        ///     Add an NLogViewerTarget for the defined log level.
        /// </summary>
        /// <param name="name">Name of the logger.</param>
        public NLogLoggingService(string name = null)
        {
            // If no name was provided, then we use the name of calling class.
            if (!string.IsNullOrWhiteSpace(name))
            {
                Name = name;
            }

            //RememberExistingFileConfiguration();
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
        /// <param name="isAsync">Will the nLog viewer target be async or not?</param>
        public NLogLoggingService(string name, string address,
            string minimumLogLevel = "debug",
            bool isAsync = true)
            : this(name)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentNullException("address");
            }

            if (string.IsNullOrWhiteSpace(minimumLogLevel))
            {
                throw new ArgumentNullException("minimumLogLevel");
            }

            ConfigureNLogViewerTarget(address, minimumLogLevel, isAsync);
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
        /// <param name="isAsync">Will the nLog viewer target be async or not?</param>
        public NLogLoggingService(string name, NLogViewerTarget target,
            string minimumLogLevel = "debug",
            bool isAsync = true)
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

            ConfigureNLogViewerTarget(target, minimumLogLevel, isAsync);
        }

        protected Logger Logger
        {
            get
            {
                // Lazy loading - no need to sync lock, cause this is only for this object-instance .. not the entire application.
                // NOTE: we need to skip 2 frames.
                //       1st frame (frame 0) == get_logger.
                //       2nd frame (frame 1) == Debug/Info/etc methods
                //       3rd frame (frame 2) == <light weight .. no idea what this is>
                return _log ??
                       (_log =
                           (string.IsNullOrEmpty(Name)
                               ? NLogExtensions.GetCurrentClassLogger(2)
                               : LogManager.GetLogger(Name)));
            }
        }

        #region Implementation of ILoggingService

        public string Name { get; private set; }

        public void Trace(string message)
        {
            if (Logger != null && Logger.IsTraceEnabled)
            {
                Logger.Trace(message);
            }
        }

        public void Trace(string message, params object[] args)
        {
            if (Logger != null && Logger.IsTraceEnabled)
            {
                Logger.Trace(message, args);
            }
        }

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

        public void Error(Exception exception, string message = null, bool isStackTraceIncluded = true)
        {
            if (Logger == null || !Logger.IsErrorEnabled)
            {
                return;
            }

            string errorMessage = string.Format("{0}{1}{2}{3}",
                string.IsNullOrWhiteSpace(message)
                    ? string.Empty
                    : message,
                string.IsNullOrWhiteSpace(message)
                    ? string.Empty
                    : " Exception Message: ",
                exception.Message,
                isStackTraceIncluded
                    ? string.Format(". Stack Trace: {0}", exception.StackTrace)
                    : string.Empty);

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

        public void Fatal(Exception exception, string message = null, bool isStackTraceIncluded = true)
        {
            string errorMessage = string.Format("{0}{1}{2}{3}",
                string.IsNullOrWhiteSpace(message)
                    ? string.Empty
                    : message,
                string.IsNullOrWhiteSpace(message)
                    ? string.Empty
                    : " Exception Message: ",
                exception.Message,
                isStackTraceIncluded
                    ? string.Format(". Stack Trace: {0}", exception.StackTrace)
                    : string.Empty);

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
        /// <param name="isAsync">Will the nLog viewer target be async or not?</param>
        /// <see cref="https://github.com/nlog/NLog/wiki/NLogViewer-target" />
        public void ConfigureNLogViewerTarget(string address,
            string minimumLogLevel = "debug",
            bool isAsync = true)
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
                Name = "SimpleLogging-nLogViewerTarget-" + DateTime.Now.Ticks,
                Address = address,
                IncludeNLogData = false
            };

            ConfigureNLogViewerTarget(target, minimumLogLevel, isAsync);
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
        /// <param name="isAsync">Will the nLog viewer target be async or not?</param>
        /// <see cref="https://github.com/nlog/NLog/wiki/NLogViewer-target" />
        public void ConfigureNLogViewerTarget(NLogViewerTarget target,
            string minimumLogLevel = "debug",
            bool isAsync = true)
        {
            // Code Reference: http://stackoverflow.com/questions/7471490/add-enable-and-disable-nlog-loggers-programmatically

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (string.IsNullOrWhiteSpace(minimumLogLevel))
            {
                throw new ArgumentNullException("minimumLogLevel");
            }

            if (LogManager.Configuration == null)
            {
                LogManager.Configuration = new LoggingConfiguration();
            }

            Target loggingRuleTarget;

            if (isAsync)
            {
                var originalTargetName = string.IsNullOrWhiteSpace(target.Name)
                        ? "unamed"
                        : target.Name;
                
                // Just trying to stick with the naming conventions.
                target.Name = string.Format("{0}_Wrapped", originalTargetName);

                // We need to make sure we wrap the target in an Async Target Wrapper.
                loggingRuleTarget = new AsyncTargetWrapper(target)
                {
                    Name = originalTargetName
                };
                LogManager.Configuration.AddTarget(originalTargetName, loggingRuleTarget);
            }
            else
            {
                loggingRuleTarget = target;
            }

            LogLevelType logLevel;
            Enum.TryParse(minimumLogLevel, true, out logLevel);

            var loggingRule = new LoggingRule("*", TryParseToLogLevel(logLevel), loggingRuleTarget);
            LogManager.Configuration.LoggingRules.Add(loggingRule);

            // Make sure all the loggers are refreshed and re-configured nicely.
            LogManager.ReconfigExistingLoggers();
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