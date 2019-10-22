using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The <see cref="ILogger"/> that logs to the application console.
    /// </summary>
    public class ConsoleLogger : BaseLogger<ConsoleLoggerSettings>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogger"/> class.
        /// </summary>
        /// <param name="settings">The console logger settings.</param>
        public ConsoleLogger(ConsoleLoggerSettings settings)
            : base(settings)
        {
        }

        #endregion

        #region BaseLogger implementation

        /// <summary>
        /// Handles the logged message being passed in.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The level of the message being logged</param>
        /// <param name="time">The time when the logging happened.</param>
        /// <param name="origin">The method/function this message was logged in</param>
        /// <param name="filePath">The code filename that this message was logged from</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from</param>
        public override void Log(string message, LogLevel level, DateTimeOffset time, string origin, string filePath, int lineNumber)
        {
            // Save old color
            var consoleOldColor = Console.ForegroundColor;

            // Default log color value 
            var consoleColor = ConsoleColor.White;

            // Color console based on level
            switch (level)
            {
                // Debug is blue
                case LogLevel.Debug:
                    consoleColor = ConsoleColor.DarkCyan;
                    break;

                // Info is gray
                case LogLevel.Info:
                    consoleColor = ConsoleColor.Gray;
                    break;

                // Warning is yellow
                case LogLevel.Warning:
                    consoleColor = ConsoleColor.Yellow;
                    break;

                // Error is  dark red
                case LogLevel.Error:
                    consoleColor = ConsoleColor.DarkRed;
                    break;

                // Fatal is red
                case LogLevel.Fatal:
                    consoleColor = ConsoleColor.Red;
                    break;
            }

            // Set the desired console color
            Console.ForegroundColor = consoleColor;

            // Compose the final message
            var finalMessage = ComposeFinalMessage(message, level, time, origin, filePath, lineNumber);

            // Write the final message to console
            Console.WriteLine(finalMessage);

            // Reset color to the original one
            Console.ForegroundColor = consoleOldColor;
        }

        #endregion
    }
}
