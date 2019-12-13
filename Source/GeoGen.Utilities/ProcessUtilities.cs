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
        /// A helper method that runs a given command with arguments asynchronously.
        /// </summary>
        /// <param name="command">The command to be run.</param>
        /// <param name="arguments">The arguments of the command.</param>
        /// <returns>The exit code, the output from the standard output stream, the output from the standard error stream.</returns>
        public static Task<(int exitCode, string output, string error)> RunCommandAsync(string command, string arguments)
        {
            // Prepare the task completion source that will indicate the end of the command and hold the exit code
            var taskCompletionSource = new TaskCompletionSource<(int exitCode, string output, string error)>();

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
                // Get the standard output 
                var standardOutput = process.StandardOutput.ReadToEnd();

                // Get the error output
                var errorOutput = process.StandardError.ReadToEnd();

                // Set the result
                taskCompletionSource.SetResult((process.ExitCode, standardOutput, errorOutput));

                // Dispose the process
                process.Dispose();
            };

            // Start the process
            process.Start();

            // Return the task represented by this asynchronous operation
            return taskCompletionSource.Task;
        }
    }
}
