using System;

namespace GeoGen.DrawingLauncher
{
    /// <summary>
    /// Represents an exception that is thrown when an execution of a command yielded a non-zero exit code.
    /// </summary>
    public class CommandException : DrawingLauncherException
    {
        #region Public properties

        /// <summary>
        /// The actual command that was run.
        /// </summary>
        public string CommandWithArguments { get; }

        /// <summary>
        /// The exit code of the command that failed.
        /// </summary>
        public int ExitCode { get; }

        /// <summary>
        /// The output from the standard process' output stream.
        /// </summary>
        public string StandardOutput { get; }

        /// <summary>
        /// The output from the standard process' error stream.
        /// </summary>
        public string ErrorOutput { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandException"/> class.
        /// </summary>
        /// <param name="message">The message containing custom information about what happened.</param>
        /// <param name="commandWithArguments">The actual command that was run.</param>
        /// <param name="exitCode">The exit code of the command that failed.</param>
        /// <param name="standardOutput">The output from the standard process' output stream.</param>
        /// <param name="errorOutput">The output from the standard process' error stream.</param>
        public CommandException(string message, string commandWithArguments, int exitCode, string standardOutput, string errorOutput)
            : base(message)
        {
            ExitCode = exitCode;
            StandardOutput = standardOutput;
            ErrorOutput = errorOutput;
            CommandWithArguments = commandWithArguments ?? throw new ArgumentNullException(nameof(commandWithArguments));
        }

        #endregion
    }
}
