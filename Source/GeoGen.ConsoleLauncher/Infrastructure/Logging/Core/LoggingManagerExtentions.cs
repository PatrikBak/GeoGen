using System.Runtime.CompilerServices;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Extension methods for <see cref="ILoggingManager"/>.
    /// </summary>
    public static class LoggingManagerExtentions
    {
        /// <summary>
        /// Logs the specific debug message to all loggers.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        public static void LogDebug(this ILoggingManager manager, string message, string origin = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            // Call the general log method 
            manager.Log(message, LogLevel.Debug, origin, filePath, lineNumber);
        }

        /// <summary>
        /// Logs the specific info message to all loggers.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        public static void LogInfo(this ILoggingManager manager, string message, string origin = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            // Call the general log method 
            manager.Log(message, LogLevel.Info, origin, filePath, lineNumber);
        }

        /// <summary>
        /// Logs the specific warning message to all loggers.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        public static void LogWarning(this ILoggingManager manager, string message, string origin = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            // Call the general log method 
            manager.Log(message, LogLevel.Warning, origin, filePath, lineNumber);
        }

        /// <summary>
        /// Logs the specific error message to all loggers.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        public static void LogError(this ILoggingManager manager, string message, string origin = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            // Call the general log method 
            manager.Log(message, LogLevel.Error, origin, filePath, lineNumber);
        }

        /// <summary>
        /// Logs the specific fatal message to all loggers.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        public static void LogFatal(this ILoggingManager manager, string message, string origin = "", [CallerFilePath]string filePath = "", [CallerLineNumber]int lineNumber = 0)
        {
            // Call the general log method 
            manager.Log(message, LogLevel.Fatal, origin, filePath, lineNumber);
        }
    }
}
