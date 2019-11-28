using GeoGen.Constructor;
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
        /// The settings for the <see cref="GeometryConstructor"/> used in the algorithm.
        /// </summary>
        public GeometryConstructorSettings GeometryConstructorSettings { get; }

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
        /// <param name="geometryConstructorSettings">The settings for the <see cref="GeometryConstructor"/> used in the algorithm.</param>
        /// <param name="theoremFindingSettings">The settings related to theorem finding.</param>
        /// <param name="theoremRankingSettings">The settings related to theorem ranking.</param>
        public AlgorithmSettings(GeometryConstructorSettings geometryConstructorSettings,
                                 TheoremFindingSettings theoremFindingSettings,
                                 TheoremRankingSettings theoremRankingSettings)
        {
            GeometryConstructorSettings = geometryConstructorSettings ?? throw new ArgumentNullException(nameof(geometryConstructorSettings));
            TheoremFindingSettings = theoremFindingSettings ?? throw new ArgumentNullException(nameof(theoremFindingSettings));
            TheoremRankingSettings = theoremRankingSettings ?? throw new ArgumentNullException(nameof(theoremRankingSettings));
        }

        #endregion
    }
}
