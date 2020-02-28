using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The default implementation of <see cref="IRankedTheoremWriter"/>.
    /// </summary>
    public class RankedTheoremWriter : IRankedTheoremWriter
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
                    $" - total ranking {rankedTheorem.Ranking.TotalRanking.ToStringWithDecimalDot()}\n");

                // Add individual rankings ordered by the total contribution (ASC) and then the aspect name
                rankedTheorem.Ranking.Rankings.OrderBy(pair => (-pair.Value.Contribution, pair.Key.ToString()))
                        // Add each on an individual line with info about the weight
                        .ForEach(pair => streamWriter.WriteLine($"  {pair.Key,-25}weight = {pair.Value.Weight.ToStringWithDecimalDot(),-10}" +
                            // The ranking
                            $"ranking = {pair.Value.Ranking.ToStringWithDecimalDot(),-10}" +
                            // And the total contribution
                            $"contribution = {pair.Value.Contribution.ToStringWithDecimalDot(),-10}"));


                // Make a new line
                streamWriter.WriteLine();
            });

            // Flush the writer
            streamWriter.Flush();
        }
    }
}