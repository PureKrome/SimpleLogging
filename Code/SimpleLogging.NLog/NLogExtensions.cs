using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NLog;

namespace SimpleLogging.NLog
{
    public static class NLogExtensions
    {
        /// <summary>
        /// Gets the logger named after the currently-being-initialized class.
        /// </summary>
        /// <returns>The logger.</returns>
        /// <remarks>This is a slow-running method. 
        /// Make sure you're not doing this in a loop.</remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Logger GetCurrentClassLogger(int framesToSkip = 1)
        {
            string loggerName = null;
            Type declaringType;

            do
            {
                var frame = new StackFrame(framesToSkip, false);
                var method = frame.GetMethod();
                if (method == null)
                {
                    break;
                }

                declaringType = method.DeclaringType;
                if (declaringType != null &&
                    !string.IsNullOrWhiteSpace(declaringType.Namespace) &&
                    declaringType.Namespace != "SimpleLogging.NLog" &&
                    !declaringType.Namespace.StartsWith("System."))
                {
                    loggerName = declaringType.FullName;
                }
                else
                {
                    framesToSkip++;
                }
            } while (string.IsNullOrWhiteSpace(loggerName) &&
                     (declaringType == null ||
                      !declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase)));

            return LogManager.GetLogger(loggerName);
        }
    }
}