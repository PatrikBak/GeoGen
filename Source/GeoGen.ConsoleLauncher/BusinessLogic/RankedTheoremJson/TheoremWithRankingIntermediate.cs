using GeoGen.Core;
using GeoGen.TheoremRanker;
using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a <see cref="TheoremWithRanking"/> as an intermediate object where complex objects 
    /// <see cref="Configuration"/> and <see cref="Theorem"/> are serialized as strings. They are converted
    /// to them using <see cref="OutputFormatter"/> and then parsed using <see cref="Parser"/>.
    /// </summary>
    public class TheoremWithRankingIntermediate
    {
        #region Public properties

        /// <summary>
        /// The string of the actual theorem that was ranked.
        /// </summary>
        public string TheoremString { get; }

        /// <summary>
        /// The ranking of the <see cref="TheoremString"/> with respect to the configuration where it was discovered.
        /// </summary>
        public TheoremRanking Ranking { get; }

        /// <summary>
        /// The string of the configuration where the <see cref="TheoremString"/> holds.
        /// </summary>
        public string ConfigurationString { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremWithRankingIntermediate"/> class.
        /// </summary>
        /// <param name="theoremString">The string of the actual theorem that was ranked.</param>
        /// <param name="ranking">The ranking of the <see cref="TheoremString"/> with respect to the configuration where it was discovered.</param>
        /// <param name="configurationString">The string of the configuration where the <see cref="TheoremString"/> holds.</param>
        public TheoremWithRankingIntermediate(string theoremString, TheoremRanking ranking, string configurationString)
        {
            TheoremString = theoremString ?? throw new ArgumentNullException(nameof(theoremString));
            Ranking = ranking ?? throw new ArgumentNullException(nameof(ranking));
            ConfigurationString = configurationString ?? throw new ArgumentNullException(nameof(configurationString));
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Converts an original <see cref="TheoremWithRanking"/> object into an intermediate object to be serialized.
        /// </summary>
        /// <param name="theoremWithRanking">The object to be converted.</param>
        /// <returns>The result of the conversion.</returns>
        public static TheoremWithRankingIntermediate Convert(TheoremWithRanking theoremWithRanking)
        {
            // Prepare the formatter for the configuration
            var formatter = new OutputFormatter(theoremWithRanking.Configuration.AllObjects);

            // Format the configuration
            var configurationString = formatter.FormatConfiguration(theoremWithRanking.Configuration)
                // Replace all curly braces that are not supported by the parser (and don't matter after all)
                .Replace("{", "").Replace("}", "");

            // Format the theorem
            var theoremString = formatter.FormatTheorem(theoremWithRanking.Theorem)
                // Replace all curly braces that are not supported by the parser (and don't matter after all)
                .Replace("{", "").Replace("}", "");

            // Return the final object
            return new TheoremWithRankingIntermediate(theoremString, theoremWithRanking.Ranking, configurationString);
        }

        /// <summary>
        /// Converts a serialized <see cref="TheoremWithRankingIntermediate"/> object into an original object.
        /// </summary>
        /// <param name="theoremWithRankingIntermediate">The object to be converted.</param>
        /// <returns>The result of the conversion.</returns>
        public static TheoremWithRanking Convert(TheoremWithRankingIntermediate theoremWithRankingIntermediate)
        {
            // Parse the configuration from the lines
            var (configuration, objectNames) = Parser.ParseConfiguration(theoremWithRankingIntermediate.ConfigurationString.Split('\n'));

            // Parse the theorem (we assume all objects are named)
            var theorem = Parser.ParseTheorem(theoremWithRankingIntermediate.TheoremString, objectNames, autocreateUnnamedObjects: false);

            // Return the final object
            return new TheoremWithRanking(theorem, theoremWithRankingIntermediate.Ranking, configuration);
        }

        #endregion
    }
}
