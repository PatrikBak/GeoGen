using GeoGen.Constructor;
using GeoGen.TheoremRanker;
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
        /// The settings for <see cref="TheoremRanker.TheoremRanker"/>.
        /// </summary>
        public TheoremRankerSettings TheoremRankerSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmSettings"/> class.
        /// </summary>
        /// <param name="geometryConstructorSettings">The settings for the <see cref="GeometryConstructor"/> used in the algorithm.</param>
        /// <param name="theoremFindingSettings">The settings related to theorem finding.</param>
        /// <param name="theoremRankerSettings">The settings for <see cref="TheoremRanker.TheoremRanker"/>.</param>
        public AlgorithmSettings(GeometryConstructorSettings geometryConstructorSettings,
                                 TheoremFindingSettings theoremFindingSettings,
                                 TheoremRankerSettings theoremRankerSettings)
        {
            GeometryConstructorSettings = geometryConstructorSettings ?? throw new ArgumentNullException(nameof(geometryConstructorSettings));
            TheoremFindingSettings = theoremFindingSettings ?? throw new ArgumentNullException(nameof(theoremFindingSettings));
            TheoremRankerSettings = theoremRankerSettings ?? throw new ArgumentNullException(nameof(theoremRankerSettings));
        }

        #endregion
    }
}
