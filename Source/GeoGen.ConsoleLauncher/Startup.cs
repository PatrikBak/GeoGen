using Ninject;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using static GeoGen.ConsoleLauncher.Log;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The entry point of the application.
    /// </summary>
    public static class Startup
    {
        #region Main method

        /// <summary>
        /// The main function.
        /// </summary>
        private static void Main()
        {
            // This makes sure that doubles in the VS debugger will be displayed with a decimal point
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            // Initialize the IoC system
            RunTaskAndHandleExceptions(() => IoC.InitializeAsync(), exitOnException: true);

            // Run the algorithm
            RunTaskAndHandleExceptions(() => IoC.Kernel.Get<IBatchRunner>().FindAllInputFilesAndRunAlgorithmsAsync());

            // Log that we're done
            LoggingManager.LogInfo("The application has finished.\n");
        }

        #endregion

        #region RunAndHandleExceptions methods

        /// <summary>
        /// Runs the given task and handles all possible exception it may produce.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="exitOnException">Indicates whether we should exist when an exception occurs.</param>
        private static void RunTaskAndHandleExceptions(Func<Task> task, bool exitOnException = false)
        {
            try
            {
                // Run the task and wait for the result
                // 
                // NOTE: Because of calling GetAwaiter().GetResult() instead of Wait(),
                //       the final exception won't be of the type AggregateException
                //
                task().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                // Log it
                LoggingManager.LogFatal($"An unexpected exception has occurred: \n\n{e}\n");

                // If we should terminate, do so
                if (exitOnException)
                    Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Runs the given action and handles all possible exception it may produce.
        /// </summary>
        /// <param name="action">The action.</param>
        private static void RunActionAndHandleExceptions(Action action)
        {
            // Fake the action as a task
            RunTaskAndHandleExceptions(() =>
            {
                // Run it
                action();

                // And return a completed task
                return Task.CompletedTask;
            });
        }

        #endregion
    }
}