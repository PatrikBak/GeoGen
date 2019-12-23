using GeoGen.Algorithm;
using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents the settings for the services of the algorithm.
    /// </summary>
    public class AlgorithmSettings
    {
        #region Public properties

        /// <summary>
        /// The settings for the algorithm facade.
        /// </summary>
        public AlgorithmFacadeSettings AlgorithmFacadeSettings { get; }

        /// <summary>
        /// The settings for the finder of best theorems.
        /// </summary>
        public BestTheoremsFinderSettings BestTheoremsFinderSettings { get; }

        /// <summary>
        /// The settings related to theorem finding.
        /// </summary>
        public TheoremFindingSettings TheoremFindingSettings { get; }

        /// <summary>
        /// The settings related to theorem ranking.
        /// </summary>
        public TheoremRankingSettings TheoremRankingSettings { get; }

        /// <summary>
        /// The settings related to the generator module.
        /// </summary>
        public GenerationSettings GenerationSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmSettings"/> class.
        /// </summary>
        /// <param name="algorithmFacadeSettings">The settings for the algorithm facade.</param>
        /// <param name="bestTheoremsFinderSettings">The settings for the finder of best theorems.</param>
        /// <param name="theoremFindingSettings">The settings related to theorem finding.</param>
        /// <param name="theoremRankingSettings">The settings related to theorem ranking.</param>
        /// <param name="generationSettings">The settings related to the generator module.</param>
        public AlgorithmSettings(AlgorithmFacadeSettings algorithmFacadeSettings,
                                 BestTheoremsFinderSettings bestTheoremsFinderSettings,
                                 TheoremFindingSettings theoremFindingSettings,
                                 TheoremRankingSettings theoremRankingSettings,
                                 GenerationSettings generationSettings)
        {
            AlgorithmFacadeSettings = algorithmFacadeSettings ?? throw new ArgumentNullException(nameof(algorithmFacadeSettings));
            BestTheoremsFinderSettings = bestTheoremsFinderSettings ?? throw new ArgumentNullException(nameof(bestTheoremsFinderSettings));
            TheoremFindingSettings = theoremFindingSettings ?? throw new ArgumentNullException(nameof(theoremFindingSettings));
            TheoremRankingSettings = theoremRankingSettings ?? throw new ArgumentNullException(nameof(theoremRankingSettings));
            GenerationSettings = generationSettings ?? throw new ArgumentNullException(nameof(generationSettings));
        }

        #endregion
    }
}
