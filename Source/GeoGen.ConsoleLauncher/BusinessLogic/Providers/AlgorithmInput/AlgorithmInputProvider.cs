using GeoGen.Algorithm;
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
    /// The default implementation of <see cref="IAlgorithmInputProvider"/> loading 
    /// data from the file system.
    /// </summary>
    public class AlgorithmInputProvider : IAlgorithmInputProvider
    {
        #region Dependencies

        /// <summary>
        /// The parser of input files.
        /// </summary>
        private readonly IParser _parser;

        #endregion

        #region Private fields

        /// <summary>
        /// The settings for the folder with inputs.
        /// </summary>
        private readonly InputFolderSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmInputProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings for the folder with inputs.</param>
        /// <param name="parser">The parser of input files.</param>
        public AlgorithmInputProvider(InputFolderSettings settings, IParser parser)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        #endregion

        #region IAlgorithmInputProvider implementation

        /// <summary>
        /// Gets algorithm inputs.
        /// </summary>
        /// <returns>The algorithm inputs.</returns>
        public async Task<IReadOnlyList<LoadedAlgorithmInput>> GetAlgorithmInputsAsync()
        {
            // Log that we're starting
            LoggingManager.LogInfo($"Starting to search for input files in {_settings.InputFolderPath}");

            // Prepare the available input files
            var inputFiles = Directory.EnumerateFiles(_settings.InputFolderPath, $"*.{_settings.FilesExtention}", SearchOption.AllDirectories)
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
                LoggingManager.LogWarning("No file found on which we could run the algorithm.");

                // Finish the method
                return new List<LoadedAlgorithmInput>();
            }

            // Inform about the found ones
            LoggingManager.LogInfo($"Found {inputFiles.Count} input file{(inputFiles.Count == 1 ? "" : "s")}:\n\n{inputFiles.Select(file => $"   - {file.path}").ToJoinedString("\n")}\n");

            // Prepare the result
            var result = new List<LoadedAlgorithmInput>();

            // Go through all the input files
            foreach (var (path, id) in inputFiles)
            {
                // Log the file being processed
                LoggingManager.LogDebug($"Processing input file {path}.");

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
                catch (Exception)
                {
                    // Log the exception
                    LoggingManager.LogError($"Couldn't read the input file {path}.");

                    // Throw further
                    throw;
                }

                #endregion

                #region Parsing it

                // Prepare an algorithm input
                var algorithmInput = default(AlgorithmInput);

                try
                {
                    // Try to parse it
                    algorithmInput = _parser.ParseInput(fileContent);
                }
                catch (ParserException)
                {
                    // Log the exception
                    LoggingManager.LogError($"Couldn't parse the input file {path}.");

                    // Throw further
                    throw;
                }

                #endregion

                // Add the loaded input to the result list
                result.Add(new LoadedAlgorithmInput
                (
                    filePath: path,
                    id: id,
                    constructions: algorithmInput.Constructions,
                    initialConfiguration: algorithmInput.InitialConfiguration,
                    numberOfIterations: algorithmInput.NumberOfIterations
                ));
            }

            // Return the found results
            return result;
        }

        #endregion
    }
}
