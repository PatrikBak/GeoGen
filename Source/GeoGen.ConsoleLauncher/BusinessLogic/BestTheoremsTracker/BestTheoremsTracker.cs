using GeoGen.Core;
using GeoGen.TheoremRanker;
using GeoGen.Utilities;
using System;
using System.IO;
using System.Linq;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The default implementation of <see cref="IBestTheoremsTracker"/> that writes results to a file.
    /// This file will be deleted at the initialization. Any time we find the best theorem, the file is 
    /// being rewritten. 
    /// </summary>
    public class BestTheoremsTracker : IBestTheoremsTracker
    {
        #region Private fields

        /// <summary>
        /// The settings for the tracker.
        /// </summary>
        private readonly BestTheoremsTrackerSettings _settings;

        /// <summary>
        /// The ladder of best theorems.
        /// </summary>
        private readonly RankingLadder<(Configuration configuration, Theorem theorem, string id), TheoremRanking> _ladder;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BestTheoremsTracker"/> class.
        /// </summary>
        /// <param name="settings">The settings for the tracker.</param>
        public BestTheoremsTracker(BestTheoremsTrackerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));

            // Delete the theorem file
            File.Delete(settings.TheoremFilePath);

            // Initialize the ladder with the requested capacity
            _ladder = new RankingLadder<(Configuration configuration, Theorem theorem, string id), TheoremRanking>(capacity: settings.NumberOfTheorems);

            // Make sure the changes are written to it
            _ladder.ContentChanged += RewriteTheoremFile;
        }

        #endregion

        #region IBestTheoremsTracker implementation

        /// <summary>
        /// Adds a given theorem holding in a given configuration with a given rank. 
        /// </summary>
        /// <param name="theorem">The theorem to potentially be tracked.</param>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <param name="rank">The rank of the theorem.</param>
        /// <param name="id">The id of the theorem.</param>
        public void AddTheorem(Theorem theorem, Configuration configuration, TheoremRanking rank, string id)
        {
            // Add the theorem to the ladder
            _ladder.Add((configuration, theorem, id), rank);

            // Its ContentChanged event will take care of the rest
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Rewrites the content of the theorem file based on the <see cref="_ladder"/>
        /// </summary>
        private void RewriteTheoremFile()
        {
            // Open the stream writer for the file
            using var streamWriter = new StreamWriter(_settings.TheoremFilePath);

            // Go through the best theorems
            _ladder.ForEach((item, index) =>
            {
                // Deconstruct
                var ((configuration, theorem, id), rank) = item;

                // Prepare the formatter of the configuration
                var formatter = new OutputFormatter(configuration.AllObjects);

                // Prepare the header
                var header = $"Theorem {index + 1} ({id})";

                // Write header
                streamWriter.WriteLine(new string('-', header.Length));
                streamWriter.WriteLine($"{header}");
                streamWriter.WriteLine(new string('-', header.Length));
                streamWriter.WriteLine();

                // Write configuration
                streamWriter.WriteLine(formatter.FormatConfiguration(configuration));

                // Write theorem
                streamWriter.WriteLine($"\n{formatter.FormatTheorem(theorem)} - total ranking {rank.TotalRanking.ToString("G5")}\n");

                // Add individual rankings ordered by the total contribution (ASC) and then the aspect name
                rank.Ranking.OrderBy(pair => (-pair.Value.coefficient * pair.Value.ranking, pair.Key.ToString()))
                    // Add each on an individual line with info about the coefficient
                    .ForEach(pair => streamWriter.WriteLine($"{pair.Key,-25}coefficient = {pair.Value.coefficient.ToString("G5"),-10}" +
                        // The ranking and the total contribution of this aspect
                        $"ranking = {pair.Value.ranking.ToString("G5"),-10}contribution = {(pair.Value.coefficient * pair.Value.ranking).ToString("G5")}"));

                // Make a new line
                streamWriter.WriteLine();
            });

            // Flush the writer
            streamWriter.Flush();
        }

        #endregion
    }
}
