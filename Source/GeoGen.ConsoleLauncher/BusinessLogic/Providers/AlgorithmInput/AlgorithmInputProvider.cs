using GeoGen.Algorithm;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        public AlgorithmInputProvider(InputFolderSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
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
            LoggingManager.LogInfo($"Found {inputFiles.Count} input file{(inputFiles.Count == 1 ? "" : "s")}:\n\n" +
                // With the description of their paths and indices
                $"{inputFiles.Select((file, index) => $"   {index + 1}. {file.path}").ToJoinedString("\n")}\n");

            // Prepare the result
            var result = new List<LoadedAlgorithmInput>();

            // Go through all the input files
            foreach (var (path, id) in inputFiles)
            {
                #region Loading the file

                // Prepare the file content
                var fileContent = default(string);

                try
                {
                    // Get the content of the file
                    fileContent = await File.ReadAllTextAsync(path);
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
                    algorithmInput = ParseInput(fileContent);
                }
                catch (ParsingException)
                {
                    // Log the exception
                    LoggingManager.LogError($"Couldn't parse the input file {path}.");

                    // Log the content
                    LoggingManager.LogDebug($"Loaded content:\n\n{fileContent}\n");

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

        /// <summary>
        /// Parses a given content to an algorithm input.
        /// </summary>
        /// <param name="content">The content of an input file.</param>
        /// <returns>The parsed algorithm input.</returns>
        private static AlgorithmInput ParseInput(string content)
        {
            // Get the lines 
            var lines = content.Split('\n')
                // Trimmed
                .Select(line => line.Trim())
                // That are not comments or empty ones
                .Where(line => !line.StartsWith('#') && !string.IsNullOrEmpty(line))
                // As a list
                .ToList();

            // There has to be some content
            if (lines.IsEmpty())
                throw new ParsingException("No lines");

            #region Parsing iterations

            // The first line should have iterations First find the iteration line
            var iterationsNumber = Regex.Match(lines[0], "^Iterations:(.+)$")
                // Take the number from the group
                ?.Groups[1].Value.Trim()
                // If there is no match, make aware
                ?? throw new ParsingException("No line specifying the number of iterations in the form 'Iterations: {number}'");

            // Prepare the number of iterations
            var numberOfIterations = default(int);

            try
            {
                // Try to parse 
                numberOfIterations = int.Parse(iterationsNumber);

                // Make sure it's a correct value
                if (numberOfIterations < 0)
                    throw new ParsingException($"The number of iterations cannot be negative, the found value is {numberOfIterations}.");
            }
            catch (Exception e) when (e is FormatException || e is OverflowException)
            {
                // Make sure the user is aware
                throw new ParsingException($"Cannot parse the number of iterations: '{iterationsNumber}'");
            }

            #endregion

            #region Finding construction and configuration sections

            // Find the construction section starting line
            var constructionLineIndex = lines.IndexOf("Constructions:");

            // Make sure there is some
            if (constructionLineIndex == -1)
                throw new ParsingException("No line starting the constructions section in the form: 'Constructions:'");

            // Find the configuration section starting line
            var configurationLineIndex = lines.IndexOf("Initial configuration:");

            // Make sure there is some
            if (configurationLineIndex == -1)
                throw new ParsingException("No line starting the initial configuration section in the form: 'Initial configuration:'");

            // Make sure the construction line is before the configuration one
            if (constructionLineIndex > configurationLineIndex)
                throw new ParsingException("Constructions should be specified before the initial configuration.");

            #endregion

            #region Parsing constructions

            // Parse the constructions...Get the lines between the found indices
            var constructions = lines.ItemsBetween(constructionLineIndex + 1, configurationLineIndex)
                // Each line defines a construction
                .Select(Parser.ParseConstruction)
                // Enumerate 
                .ToReadOnlyHashSet();

            #endregion

            #region Parsing configuration

            // Get the configuration lines as the remaining lines
            var configurationLines = lines.ItemsBetween(configurationLineIndex + 1, lines.Count).ToList();

            // Parse the configuration from them
            var configuration = Parser.ParseConfiguration(configurationLines).configuration;

            #endregion

            // Return the final input
            return new AlgorithmInput
            (
                initialConfiguration: configuration,
                constructions: constructions,
                numberOfIterations: numberOfIterations
            );
        }

        #endregion
    }
}
