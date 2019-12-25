using System.Diagnostics;
using System.Text;
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
        /// <returns>The exit code, the output from the command's output stream, the output from the command's error stream.</returns>
        public static Task<(int exitCode, string outputData, string errorData)> RunCommandAsync(string command, string arguments)
        {
            // Prepare the task that will indicate the end of the command and hold the data
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

            // Prepare the string builders for the incoming output and error data
            var ouputData = new StringBuilder();
            var errorData = new StringBuilder();

            // Handle any incoming output and error data
            process.OutputDataReceived += (s, ea) => ouputData.Append(ea.Data);
            process.ErrorDataReceived += (s, ea) => errorData.Append(ea.Data);

            // Handle when it exists
            process.Exited += (sender, args) =>
            {
                // Set the result
                taskCompletionSource.SetResult((process.ExitCode, ouputData.ToString(), errorData.ToString()));

                // Dispose the process
                process.Dispose();
            };

            // Start the process
            process.Start();

            // Start reading the output and error streams
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Return the task represented by this asynchronous operation
            return taskCompletionSource.Task;
        }
    }
}
