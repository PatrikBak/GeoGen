using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GeoGen.Drawer
{
    /// <summary>
    /// Represents a drawer that generates MetaPost figures
    /// </summary>
    public class MetapostDrawer : IDrawer
    {
        #region Private fields

        /// <summary>
        /// The constructor that calculates coordinates for us.
        /// </summary>
        private readonly IGeometryConstructor _constructor;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MetapostDrawer"/> class.
        /// </summary>
        /// <param name="constructor">The constructor that calculates coordinates for us.</param>
        public MetapostDrawer(IGeometryConstructor constructor)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        #endregion

        #region IMetapostDrawer implementation

        /// <summary>
        /// Draws given configurations with its theorems.
        /// </summary>
        /// <param name="configurationWithTheorems">The configurations with theorems to be drawn.</param>
        /// <returns>The task representing the result.</returns>
        public async Task DrawAsync(IEnumerable<(Configuration configuration, Theorem theorem)> configurationWithTheorems)
        {
            // Create a dummy figures
            var figures = new[]
            {
                new MetapostFigure(),
                new MetapostFigure()
            };

            // Get the code for them 
            var code = CreateCode(figures);

            // Log
            Console.WriteLine("Writing MetaPost file");

            // Create the file that will be compiled
            // TODO: Settings 
            await File.WriteAllTextAsync("figures.mp", code);

            // Log
            Console.WriteLine("Running compilation");

            // Compile
            // TODO: Settings
            await RunCommandAsync("mpost", "-interaction=nonstopmode -s prologues=3 \"figures.mp\"");

            // Open them 
            for (var i = 0; i < figures.Length; i++)
            {
                // Get the input file name
                var input = $"figures.{i}";

                // Run the command
                // TODO: Settings (potential conversion to png)
                await RunCommandAsync(@"C:\Program Files\SumatraPDF\SumatraPDF.exe", $"\"{input}\"");
            }
        }

        /// <summary>
        /// The method that generates the actual MetaPost code to be complied. 
        /// </summary>
        /// <param name="figures">The figures to be drawn.</param>
        /// <param name="unit">The unit by which all the objects should be scaled.</param>
        /// <returns>A compilable MetaPost code of the figures.</returns>
        private string CreateCode(IEnumerable<MetapostFigure> figures)
        {
            // Let's use StringBuilder for efficiency
            var code = new StringBuilder();

            // Append the preamble
            // TODO: Settings
            code.Append("input macros\n\n;");

            // Declare the unit
            // TODO: Settings
            code.Append("u=8cm;\n\n");

            // Append all the figures
            figures.ForEach((figure, index) =>
            {
                // Append the preamble
                code.Append($"beginfig({index});\n\n");

                // Append the actual code
                code.Append(figure.ToCode());

                // Append the end
                code.Append($"endfig;\n\n");
            });

            // Append the end
            code.Append("end;");

            // Return the result
            return code.ToString();
        }

        /// <summary>
        /// A helper method that runs a given command with arguments asynchronously
        /// </summary>
        /// <param name="command">The command to be run.</param>
        /// <param name="arguments">The arguments of the command.</param>
        /// <returns>The task representing the result.</returns>
        private static Task RunCommandAsync(string command, string arguments)
        {
            // Prepare the task completion source that will indicate the end of the command
            var taskCompletionSource = new TaskCompletionSource<bool>();

            // Prepare the process
            var process = new Process
            {
                // Setup the start
                StartInfo =
                {
                    // Pass the command
                    FileName = command,

                    // With its arguments
                    Arguments = arguments
                },

                // This is needed for the exit event to be fired
                EnableRaisingEvents = true
            };

            // Handle when it exists
            process.Exited += (sender, args) =>
            {
                // Make sure the result is set
                taskCompletionSource.SetResult(true);

                // Dispose the process
                process.Dispose();
            };

            // Simply write all the output to the console for now
            process.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (s, e) => Console.Error.WriteLine(e.Data);

            // Start the process
            process.Start();

            // Return the task represented by this asynchronous operation
            return taskCompletionSource.Task;
        }

        #endregion
    }
}
