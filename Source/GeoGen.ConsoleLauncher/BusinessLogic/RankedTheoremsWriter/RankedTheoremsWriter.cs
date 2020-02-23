using GeoGen.Algorithm;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The default implementation of <see cref="IRankedTheoremsWriter"/>.
    /// </summary>
    public class RankedTheoremsWriter : IRankedTheoremsWriter
    {
        /// <summary>
        /// Writes given ranked theorems to a file.
        /// </summary>
        /// <param name="theorems">The enumerable of ranked theorems with their ids to be written to the file.</param>
        /// <param name="theoremFilePath">The path to the file where the theorems should be written. </param>
        public void WriteTheorems(IEnumerable<(TheoremWithRanking rankedTheorem, string id)> theorems, string theoremFilePath)
        {
            // Open the stream writer for the file
            using var streamWriter = new StreamWriter(theoremFilePath);

            // Go through the theorems
            theorems.ForEach((pair, index) =>
            {
                // Deconstruct
                var (rankedTheorem, id) = pair;

                // Prepare the formatter of the configuration
                var formatter = new OutputFormatter(rankedTheorem.Configuration.AllObjects);

                // Prepare the header
                var header = $"Theorem {index + 1} ({id})";

                // Write header
                streamWriter.WriteLine(new string('-', header.Length));
                streamWriter.WriteLine($"{header}");
                streamWriter.WriteLine(new string('-', header.Length));
                streamWriter.WriteLine();

                // Write configuration
                streamWriter.WriteLine(formatter.FormatConfiguration(rankedTheorem.Configuration));

                // Write the theorem
                streamWriter.WriteLine($"\n{formatter.FormatTheorem(rankedTheorem.Theorem)}" +
                    // With the ranking
                    $" - total ranking {rankedTheorem.Ranking.TotalRanking.ToString("G5")} " +
                    // And with the fact whether it has been simplified
                    $"{(rankedTheorem.IsSimplified ? "(simplified)" : "")}\n");

                // Add individual rankings ordered by the total contribution (ASC) and then the aspect name
                rankedTheorem.Ranking.Ranking.OrderBy(pair => (-pair.Value.Coefficient * pair.Value.Ranking, pair.Key.ToString()))
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
    }
}