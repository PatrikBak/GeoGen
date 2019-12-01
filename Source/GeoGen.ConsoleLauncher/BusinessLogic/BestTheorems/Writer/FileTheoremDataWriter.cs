using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a <see cref="ITheoremDataWriter"/> that writes theorems to a file. 
    /// </summary>
    public class FileTheoremDataWriter : ITheoremDataWriter
    {
        #region Private fields

        /// <summary>
        /// The settings for the writer.
        /// </summary>
        private readonly FileTheoremDataWriterSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTheoremDataWriter"/> class.
        /// </summary>
        /// <param name="settings">The settings for the writer.</param>
        public FileTheoremDataWriter(FileTheoremDataWriterSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region ITheoremWriter implementation

        /// <summary>
        /// Writes given theorems in this order.
        /// </summary>
        /// <param name="theorems">The enumerable of theorem data.</param>
        public void WriteTheorems(IEnumerable<TheoremData> theorems)
        {
            // Open the stream writer for the file
            using var streamWriter = new StreamWriter(_settings.TheoremFilePath);

            // Go through the theorems
            theorems.ForEach((data, index) =>
            {
                // Prepare the formatter of the configuration
                var formatter = new OutputFormatter(data.Configuration.AllObjects);

                // Prepare the header
                var header = $"Theorem {index + 1} ({data.Id})";

                // Write header
                streamWriter.WriteLine(new string('-', header.Length));
                streamWriter.WriteLine($"{header}");
                streamWriter.WriteLine(new string('-', header.Length));
                streamWriter.WriteLine();

                // Write configuration
                streamWriter.WriteLine(formatter.FormatConfiguration(data.Configuration));

                // Write theorem
                streamWriter.WriteLine($"\n{formatter.FormatTheorem(data.Theorem)} - total ranking {data.Ranking.TotalRanking.ToString("G5")}\n");

                // Add individual rankings ordered by the total contribution (ASC) and then the aspect name
                data.Ranking.Ranking.OrderBy(pair => (-pair.Value.Coefficient * pair.Value.Ranking, pair.Key.ToString()))
                    // Add each on an individual line with info about the coefficient
                    .ForEach(pair => streamWriter.WriteLine($"{pair.Key,-25}coefficient = {pair.Value.Coefficient.ToString("G5"),-10}" +
                        // The ranking, the total contribution of this aspect, and the message
                        $"contribution = {(pair.Value.Coefficient * pair.Value.Ranking).ToString("G5"),-10}ranking = {pair.Value.Ranking.ToString("G5"),-10}{pair.Value.Message}"));

                // Make a new line
                streamWriter.WriteLine();
            });

            // Flush the writer
            streamWriter.Flush();
        }

        #endregion
    }
}