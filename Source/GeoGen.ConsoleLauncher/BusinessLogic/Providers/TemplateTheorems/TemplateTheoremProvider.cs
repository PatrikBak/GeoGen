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
        #region Dependencies

        /// <summary>
        /// The parser of theorems.
        /// </summary>
        private readonly IParser _parser;

        #endregion

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
        /// <param name="parser">The parser of theorems.</param>
        public TemplateTheoremProvider(TemplateTheoremsFolderSettings settings, IParser parser)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        }

        #endregion

        #region ITemplateTheoremProvider implementation

        /// <summary>
        /// Gets template theorems.
        /// </summary>
        /// <returns>The template theorems.</returns>
        public async Task<List<TemplateTheorem>> GetTemplateTheoremsAsync()
        {
            // Log that we're starting
            LoggingManager.LogInfo($"Starting to search for template theorems in {_settings.TheoremsFolderPath}");

            // Prepare the result
            var result = new List<TemplateTheorem>();

            // Go through all the files in the template theorems folder with the requested extension
            foreach (var path in Directory.EnumerateFiles(_settings.TheoremsFolderPath, $"*.{_settings.FilesExtention}"))
            {
                // Log the current file
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

                    // Skip this file
                    continue;
                }

                #endregion

                #region Parsing theorems file

                try
                {
                    // Try to parse it
                    var theorems = _parser.ParseTheorems(fileContent)
                        // Cast each theorem to a template theorem
                        .Select((theorem, i) => new TemplateTheorem(theorem, Path.GetFileNameWithoutExtension(path), i + 1));

                    // Add all of them to our result
                    result.AddRange(theorems);
                }
                catch (ParserException e)
                {
                    // Log the exception
                    LoggingManager.LogError($"Couldn't parse the input file {path}: {e.Message}");

                    // Skip this file
                    continue;
                }

                #endregion
            }

            // Return the result
            return result;
        }

        #endregion
    }
}
