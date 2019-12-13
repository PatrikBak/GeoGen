using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GeoGen.Utilities
{
    /// <summary>
    /// The utilities related to running <see cref="Process"/>es.
    /// </summary>
    public static class ProcessUtilities
    {
        /// <summary>
        /// A helper method that runs a given command with arguments asynchronously and returns the exit code.
        /// </summary>
        /// <param name="command">The command to be run.</param>
        /// <param name="arguments">The arguments of the command.</param>
        /// <param name="outputDataHandler">The handler for output data of the process' standard output stream.</param>
        /// <param name="errorDataHandler">The handler for error data of the process' standard output stream.</param>
        /// <returns>The exit code.</returns>
        public static Task<int> RunCommandAsync(string command, string arguments, Action<string> outputDataHandler = null, Action<string> errorDataHandler = null)
        {
            // Prepare the task completion source that will indicate the end of the command and hold the exit code
            var taskCompletionSource = new TaskCompletionSource<int>();

            // Prepare the process
            var process = new Process
            {
                // Setup the start
                StartInfo =
                {
                    // Pass the command
                    FileName = command,

                    // With its arguments
                    Arguments = arguments,

                    // Redirect the output
                    RedirectStandardOutput = true,

                    // Redirect the errors
                    RedirectStandardError = true
                },

                // This is needed for the exit event to be fired
                EnableRaisingEvents = true
            };

            // Handle when it exists
            process.Exited += (sender, args) =>
            {
                // Make sure the result is set
                taskCompletionSource.SetResult(process.ExitCode);

                // Dispose the process
                process.Dispose();
            };

            // Handle the data using the provided handlers
            process.OutputDataReceived += (s, e) =>
            {
                // If the data isn't data (which surprisingly could happen), write them
                if (e.Data != null)
                    outputDataHandler?.Invoke(e.Data);
            };
            process.ErrorDataReceived += (s, e) =>
            {
                // If the data isn't data (which surprisingly could happen), write them
                if (e.Data != null)
                    errorDataHandler?.Invoke(e.Data);
            };

            // Start the process
            process.Start();

            // Start reading the output and error stream
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Return the task represented by this asynchronous operation
            return taskCompletionSource.Task;
        }
    }
}
