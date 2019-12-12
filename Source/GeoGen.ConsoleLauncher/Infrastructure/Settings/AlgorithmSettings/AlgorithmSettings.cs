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
        /// The settings related to theorem finding.
        /// </summary>
        public TheoremFindingSettings TheoremFindingSettings { get; }

        /// <summary>
        /// The settings related to theorem ranking.
        /// </summary>
        public TheoremRankingSettings TheoremRankingSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmSettings"/> class.
        /// </summary>
        /// <param name="algorithmFacadeSettings">The settings for the algorithm facade.</param>
        /// <param name="theoremFindingSettings">The settings related to theorem finding.</param>
        /// <param name="theoremRankingSettings">The settings related to theorem ranking.</param>
        public AlgorithmSettings(AlgorithmFacadeSettings algorithmFacadeSettings,
                                 TheoremFindingSettings theoremFindingSettings,
                                 TheoremRankingSettings theoremRankingSettings)
        {
            AlgorithmFacadeSettings = algorithmFacadeSettings ?? throw new ArgumentNullException(nameof(algorithmFacadeSettings));
            TheoremFindingSettings = theoremFindingSettings ?? throw new ArgumentNullException(nameof(theoremFindingSettings));
            TheoremRankingSettings = theoremRankingSettings ?? throw new ArgumentNullException(nameof(theoremRankingSettings));
        }

        #endregion
    }
}
