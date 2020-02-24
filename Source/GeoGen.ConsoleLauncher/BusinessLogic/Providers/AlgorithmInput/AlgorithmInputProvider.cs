using GeoGen.Algorithm;
using GeoGen.Core;
using GeoGen.Infrastructure;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GeoGen.Infrastructure.Log;

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
        private readonly AlgorithmInputProviderSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmInputProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings for the folder with inputs.</param>
        public AlgorithmInputProvider(AlgorithmInputProviderSettings settings)
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
            var inputFiles = Directory.EnumerateFiles(_settings.InputFolderPath, $"*.{_settings.FileExtension}", SearchOption.AllDirectories)
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
            foreach (var (inputFilePath, id) in inputFiles)
            {
                #region Loading the file

                // Prepare the file content
                var fileContent = default(string);

                try
                {
                    // Get the content of the file
                    fileContent = await File.ReadAllTextAsync(inputFilePath);
                }
                catch (Exception e)
                {
                    // If it cannot be done, make aware
                    throw new GeoGenException($"Couldn't load the input file '{inputFilePath}'", e);
                }

                #endregion

                #region Parsing it

                // Get the lines 
                var lines = fileContent.Split('\n')
                    // Trimmed
                    .Select(line => line.Trim())
                    // That are not comments or empty ones
                    .Where(line => !line.StartsWith('#') && !string.IsNullOrEmpty(line))
                    // As a list
                    .ToList();

                // If there is no content
                if (lines.IsEmpty())
                {
                    // Warn
                    LoggingManager.LogWarning($"Empty input file {inputFilePath}");

                    // Move on
                    continue;
                }

                try
                {
                    // Try to parse it
                    var algorithmInput = ParseInput(lines);

                    // Add the loaded input to the result list
                    result.Add(new LoadedAlgorithmInput
                    (
                        filePath: inputFilePath,
                        id: id,
                        constructions: algorithmInput.Constructions,
                        initialConfiguration: algorithmInput.InitialConfiguration,
                        numberOfIterations: algorithmInput.NumberOfIterations,
                        maximalNumbersOfObjectsToAdd: algorithmInput.MaximalNumbersOfObjectsToAdd
                    ));
                }
                catch (ParsingException e)
                {
                    // Log the content
                    LoggingManager.LogDebug($"Loaded content:\n\n{fileContent}\n");

                    // Throw further
                    throw new GeoGenException($"Couldn't parse the input file {inputFilePath}.", e);
                }

                #endregion
            }

            // Return the found results
            return result;
        }

        /// <summary>
        /// Parses given lines to an algorithm input.
        /// </summary>
        /// <param name="lines">The trimmed non-empty lines without comments to be parsed.</param>
        /// <returns>The parsed algorithm input.</returns>
        private static AlgorithmInput ParseInput(IReadOnlyList<string> lines)
        {
            #region Finding construction and configuration sections

            // Make sure the first line beings constructions
            if (lines[0] != "Constructions:")
                throw new ParsingException($"The first line should be 'Constructions:' to start the constructions section");

            // Find the configuration section starting line
            var configurationLineIndex = lines.IndexOf("Initial configuration:");

            // Make sure there is some
            if (configurationLineIndex == -1)
                throw new ParsingException("There should be a line ''Initial configuration:'' starting the initial configuration");

            #endregion

            #region Parsing constructions

            // Parse the constructions...Get the lines between the first 
            // index and the one starting the configuration section
            var constructions = lines.ItemsBetween(1, configurationLineIndex)
                // Each line defines a construction
                .Select(Parser.ParseConstruction)
                // Enumerate 
                .ToReadOnlyHashSet();

            #endregion

            #region Parsing configuration

            // Get the configuration lines from the remaining lines
            var configurationLines = lines.ItemsBetween(configurationLineIndex + 1, lines.Count)
                // Unless we run into a line specifying iterations
                .TakeWhile(line => !line.StartsWith("Iterations:"))
                // Enumerate
                .ToList();

            // Parse the configuration from them
            var configuration = Parser.ParseConfiguration(configurationLines).configuration;

            #endregion

            #region Parsing iterations

            // Find the index of the iteration line, which should be after the configuration declaration
            var iterationLineIndex = configurationLineIndex + configurationLines.Count + 1;

            // Make sure we have enough lines
            if (iterationLineIndex >= lines.Count)
                throw new ParsingException("The line after the specification of the configuration should be in the form 'Iterations: {number}'");

            // Now we can parse the line
            var iterationsNumberMatch = Regex.Match(lines[iterationLineIndex], "^Iterations:(.+)$");

            // If there is no match, make aware
            if (!iterationsNumberMatch.Success)
                throw new ParsingException("The line after the specification of the configuration should be in the form 'Iterations: {number}'");

            // Prepare the number of iterations
            var numberOfIterations = default(int);

            try
            {
                // Try to parse 
                numberOfIterations = int.Parse(iterationsNumberMatch.Groups[1].Value.Trim());

                // Make sure it's a correct value
                if (numberOfIterations < 0)
                    throw new ParsingException($"The number of iterations cannot be negative, the found value is {numberOfIterations}.");
            }
            catch (Exception e) when (e is FormatException || e is OverflowException)
            {
                // Make sure the user is aware
                throw new ParsingException($"Cannot parse the number of iterations: '{iterationsNumberMatch.Groups[1].Value.Trim()}'");
            }

            #endregion

            #region Parsing maximal number of objects to be added

            // Find the configuration object types
            var objectTypes = Enum.GetValues(typeof(ConfigurationObjectType)).Cast<ConfigurationObjectType>().ToList();

            // Each type needs to have a line. Make sure we have enough lines
            if (objectTypes.Count != lines.Count - 1 - iterationLineIndex)
                throw new ParsingException($"There should be exactly {objectTypes.Count} lines after the iterations, " +
                    $"each specifying the maximal number of objects of the given type of this configuration.");

            // Look for them
            var maximalNumbersOfObjectsToAdd = objectTypes.ToDictionary(objectType => objectType, objectType =>
            {
                // Go through the remaining lines...
                var maximalNumberMatch = lines.ItemsBetween(iterationLineIndex, lines.Count)
                    // The line should be for example 'MaximalPoints: 4'
                    .Select(line => Regex.Match(line, $"^Maximal{objectType}s:(.+)$"))
                    // Take the first match, if there is any
                    .FirstOrDefault(match => match.Success)
                    // If not, make aware
                    ?? throw new ParsingException($"No line in the form Maximal{objectType}s: {{number}}");

                // Prepare the maximal number
                var maximalNumber = default(int);

                try
                {
                    // Try to parse 
                    maximalNumber = int.Parse(maximalNumberMatch.Groups[1].Value.Trim());
                }
                catch (Exception)
                {
                    // Make sure the user is aware
                    throw new ParsingException($"Cannot parse the maximal number of {objectTypes}s: '{maximalNumberMatch.Groups[1].Value.Trim()}'");
                }

                // Make sure it's a correct value
                if (maximalNumber < 0)
                    throw new ParsingException($"The maximal number of {objectType}s cannot be negative, the found value is {maximalNumber}.");

                // Now we have it parse and we can finally return it
                return maximalNumber;
            });

            #endregion

            // Return the final input
            return new AlgorithmInput
            (
                initialConfiguration: configuration,
                constructions: constructions,
                numberOfIterations: numberOfIterations,
                maximalNumbersOfObjectsToAdd: maximalNumbersOfObjectsToAdd
            );
        }

        #endregion
    }
}
