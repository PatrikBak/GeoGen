using GeoGen.Core;
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
    /// The default implementation of <see cref="ITemplateTheoremProvider"/> loading 
    /// data from the file system.
    /// </summary>
    public class TemplateTheoremProvider : ITemplateTheoremProvider
    {
        #region Private fields

        /// <summary>
        /// The settings of the template theorems folder.
        /// </summary>
        private readonly TemplateTheoremsFolderSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateTheoremProvider"/> class.
        /// </summary>
        /// <param name="settings">The settings of the template theorems folder.</param>
        public TemplateTheoremProvider(TemplateTheoremsFolderSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region ITemplateTheoremProvider implementation

        /// <summary>
        /// Gets template theorems.
        /// </summary>
        /// <returns>The list of loaded configuration with its theorems.</returns>
        public async Task<IReadOnlyList<(Configuration, TheoremMap)>> GetTemplateTheoremsAsync()
        {
            // If the dictionary doesn't exist...
            if (!Directory.Exists(_settings.TheoremsFolderPath))
            {
                // Warn
                LoggingManager.LogWarning($"The directory for template theorems {_settings.TheoremsFolderPath} doesn't exist.");

                // Break
                return new List<(Configuration, TheoremMap)>();
            }

            // Log that we're starting
            LoggingManager.LogInfo($"Starting to search for template theorems in {_settings.TheoremsFolderPath}.");

            // Prepare the result
            var result = new List<(Configuration templateConfiguration, TheoremMap templateTheorems)>();

            // Prepare the theorem files
            // Load the theorem folders 
            var theoremFiles = Directory.EnumerateDirectories(_settings.TheoremsFolderPath)
                // Order them by names
                .OrderBy(Path.GetDirectoryName)
                // For each get the files 
                .SelectMany(folder => Directory.EnumerateFiles(folder, $"*.{_settings.FilesExtention}", SearchOption.AllDirectories));

            // Go through all the files
            foreach (var path in theoremFiles)
            {
                #region Reading theorems file

                // Prepare the content
                var fileContent = default(string);

                try
                {
                    // Get the content
                    fileContent = await File.ReadAllTextAsync(path);
                }
                catch (Exception)
                {
                    // Log the exception
                    LoggingManager.LogError($"Couldn't read the theorems file {path}.");

                    // Throw further
                    throw;
                }

                #endregion

                #region Parsing theorems file

                try
                {
                    // Try to parse it
                    var (theorems, configuration) = ParseTheoremsAndConfiguration(fileContent);

                    // Create the template theorems
                    var templateTheorems = theorems.Select((theorem, i) => new TemplateTheorem(theorem,
                                // To get file we look for relative path
                                Path.GetRelativePath(_settings.TheoremsFolderPath, path),
                                // Set the theorem number according to the file
                                i + 1));

                    // Create a map from them
                    var theoremMap = new TheoremMap(templateTheorems);

                    // If there is no theorem
                    if (theoremMap.AllObjects.IsEmpty())
                    {
                        // Log that we're skipping it
                        LoggingManager.LogWarning($"No theorems in file {path}.");

                        // Skip
                        continue;
                    }

                    // Otherwise add it to our result
                    result.Add((configuration, theoremMap));
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
            }

            #region Logging database stats

            // Find the total number of theorems
            var numberOfTheorems = result.Sum(theorem => theorem.templateTheorems.Count);

            // Log count
            LoggingManager.LogInfo($"Found {result.Count} theorem file(s) with {numberOfTheorems} theorem(s).");

            // Prepare the map of types and counts
            // Take all the theorems
            var typeToCountsString = result.SelectMany(pair => pair.templateTheorems.AllObjects)
                // Group them by their type
                .GroupBy(type => type.Type)
                // Order by the count of theorems of each type
                .OrderBy(group => group.Count())
                // Compose strings
                .Select(group => $"  - {group.Key} --> {group.Count()}")
                // Join
                .ToJoinedString("\n");

            // Log it
            LoggingManager.LogInfo($"Types of the found theorems: \n\n{typeToCountsString}\n");

            #endregion

            // Return the result
            return result;
        }

        /// <summary>
        /// Parses a given content to the list of theorems and the configuration where the theorems hold.
        /// </summary>
        /// <param name="content">The content of a file containing template theorems.</param>
        /// <returns>The parsed theorems with configuration.</returns>
        private static (List<Theorem> theorems, Configuration configuration) ParseTheoremsAndConfiguration(string content)
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

            #region Finding configuration and theorems sections

            // The first line has to be the configuration
            if (lines[0] != "Configuration:")
                throw new ParsingException("The first line should be in the form: 'Configuration:'");

            // Get the index of the theorem starting line
            var theoremsLineIndex = lines.IndexOf("Theorems:");

            // Make sure there is some
            if (theoremsLineIndex == -1)
                throw new ParsingException("No line starting the theorems section in the form: 'Theorems:'");

            #endregion

            #region Parsing configuration

            // Get the configuration lines
            var configurationLines = lines.ItemsBetween(1, theoremsLineIndex).ToList();

            // Parse the configuration
            var (configuration, namesToObjects) = Parser.ParseConfiguration(configurationLines);

            #endregion

            #region Parsing theorems

            // Get the theorem lines
            var theoremLines = lines.ItemsBetween(theoremsLineIndex + 1, lines.Count);

            // Parse the theorems from each line
            var theorems = theoremLines.Select(line => Parser.ParseTheorem(line, namesToObjects)).ToList();

            #endregion

            // Return the final result
            return (theorems, configuration);
        }

        #endregion
    }
}