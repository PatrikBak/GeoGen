namespace GeoGen.Infrastructure
{
    /// <summary>
    /// Represents a manager of all loggers. 
    /// </summary>
    public interface ILoggingManager
    {
        /// <summary>
        /// Logs the specific debug message to all loggers.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The level of the message being logged.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        void Log(string message, LogLevel level, string origin = "", string filePath = "", int lineNumber = 0);
    }
}
