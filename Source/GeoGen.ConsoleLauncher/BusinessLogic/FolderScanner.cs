using GeoGen.Core;
using GeoGen.Generator;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static GeoGen.ConsoleLauncher.Log;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The default implementation of <see cref="IFolderScanner"/>.
    /// </summary>
    public class FolderScanner : IFolderScanner
    {
        #region Dependencies

        /// <summary>
        /// The parser of input files and theorems.
        /// </summary>
        private readonly IParser _parser;

        /// <summary>
        /// The processor of the algorithm output.
        /// </summary>
        private readonly IAlgorithmRunner _runner;

        #endregion

        #region Private fields

        /// <summary>
        /// The settings for the folder structure.
        /// </summary>
        private readonly FolderSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FolderScanner"/> class.
        /// </summary>
        /// <param name="settings">The settings for the folder structure.</param>
        /// <param name="runner">The runner of the algorithm for particular input.</param>
        /// <param name="parser">The parser of input files and theorems.</param>
        public FolderScanner(FolderSettings settings, IAlgorithmRunner runner, IParser parser)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _runner = runner ?? throw new ArgumentNullException(nameof(runner));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        #endregion

        #region IFolderScanner implementation

        /// <summary>
        /// Scans the folder with input files and runs the algorithm on them.
        /// </summary>
        /// <returns>The task representing the action.</returns>
        public async Task ScanAsync()
        {
            #region Prepare template theorems

            // Find the theorems folder in the working folder
            var theoremsFolder = Path.Combine(_settings.WorkingFolder, _settings.TemplateTheoremsFolder);

            // Prepare the available input files, start in the template directory
            var templateTheorems = Directory.EnumerateFiles(theoremsFolder, $"*.{_settings.FilesExtention}")
                // Read all theorems from each of them
                .Select(async path =>
                {
                    // Log it
                    LoggingManager.LogInfo($"Processing theorems file {path}.");

                    #region Reading theorems file

                    // Prepare the content
                    var fileContent = default(string);

                    try
                    {
                        // Get the content
                        fileContent = await File.ReadAllTextAsync(path);

                        // Log it
                        LoggingManager.LogDebug($"Loaded content:\n\n{fileContent}\n");
                    }
                    catch (Exception e)
                    {
                        // Log the exception
                        LoggingManager.LogError($"Couldn't read the theorems file {path}.");

                        // Log the internal exception
                        LoggingManager.LogDebug($"{e}\n");

                        // Return no theorems
                        return new List<Theorem>();
                    }

                    #endregion

                    #region Parsing theorems file

                    try
                    {
                        // Try to parse it
                        return _parser.ParseTheorems(fileContent);
                    }
                    catch (ParserException e)
                    {
                        // Log the exception
                        LoggingManager.LogError($"Couldn't parse the input file {path}: {e.Message}");

                        // Return no theorems
                        return new List<Theorem>();
                    }

                    #endregion
                })
                // Take the task results
                .SelectMany(t => t.Result)
                // Enumerate to a list
                .ToList();

            #endregion

            #region Prepare input files

            // Prepare the available input files
            var inputFiles = Directory.EnumerateFiles(_settings.WorkingFolder, $"*.{_settings.FilesExtention}")
                    // Find their file names
                    .Select(path => (path, fileName: Path.GetFileNameWithoutExtension(path)))
                    // Take only those that start with the requested pattern
                    .Where(tuple => tuple.fileName.StartsWith(_settings.InputFilePrefix))
                    // Find their ids 
                    .Select(tuple => (tuple.path, id: tuple.fileName.Substring(_settings.InputFilePrefix.Length)))
                    // Enumerate them
                    .ToList();

            // Make sure there is some...
            if (inputFiles.Count == 0)
            {
                // Log it
                LoggingManager.LogInfo("No file found on which we could run the algorithm.");

                // Finish the method
                return;
            }

            // Inform about the found ones
            LoggingManager.LogInfo($"Found input files:\n\n{inputFiles.Select(file => $"{new string(' ', 15)}{file.path}").ToJoinedString("\n")}\n");

            #endregion

            // Sequentially run the algorithm on them
            foreach (var (path, id) in inputFiles)
            {
                // Log the file being processed
                LoggingManager.LogInfo($"Processing input file {path}.");

                #region Loading the file

                // Prepare the file content
                var fileContent = default(string);

                try
                {
                    // Get the content of the file
                    fileContent = await File.ReadAllTextAsync(path);

                    // Log the content
                    LoggingManager.LogDebug($"Loaded content:\n\n{fileContent}\n");
                }
                catch (Exception e)
                {
                    // Log the exception
                    LoggingManager.LogError($"Couldn't read the input file {path}.");

                    // Log the internal exception
                    LoggingManager.LogDebug($"{e}\n");

                    // Continue on the next file
                    continue;
                }

                #endregion

                #region Parsing it

                // Prepare the generator input
                var generatorInput = default(GeneratorInput);

                try
                {
                    // Try to parse it
                    generatorInput = _parser.ParseInput(fileContent);
                }
                catch (ParserException e)
                {
                    // Log the exception
                    LoggingManager.LogError($"Couldn't parse the input file {path}: {e.Message}");

                    // Continue on the next file
                    continue;
                }

                #endregion

                #region Running algorithm

                // Prepare the output path
                var outputPath = Path.Combine(_settings.WorkingFolder, $"{_settings.OutputFilePrefix}{id}.{_settings.FilesExtention}");

                try
                {
                    // Prepare the writer for the output
                    using (var writer = new StreamWriter(new FileStream(outputPath, FileMode.Create, FileAccess.Write)))
                    {
                        // Prepare the input
                        var input = new AlgorithmInput
                        {
                            GeneratorInput = generatorInput,
                            TemplateTheorems = templateTheorems
                        };

                        // Run the algorithm
                        _runner.Run(input, writer);
                    }
                }
                catch (Exception e)
                {
                    // Log the exception
                    LoggingManager.LogError($"Couldn't perform the algorithm on the input file {path}.");

                    // Log the internal exception
                    LoggingManager.LogDebug($"{e}\n");

                    // Continue on the next file
                    continue;
                }

                #endregion
            }
        }

        #endregion
    }
}
