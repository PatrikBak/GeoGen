using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.TheoremFinder;
using System;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents the settings for the services of the algorithm.
    /// </summary>
    public class AlgorithmSettings
    {
        #region Public properties

        /// <summary>
        /// The settings for the <see cref="Pictures"/> used in the algorithm.
        /// </summary>
        public PicturesSettings PicturesSettings { get; }

        /// <summary>
        /// The settings for the tangent circles theorem finder.
        /// </summary>
        public TangentCirclesTheoremFinderSettings TangentCirclesTheoremFinderSettings { get; }

        /// <summary>
        /// The settings for the line tangent to circle theorem finder.
        /// </summary>
        public LineTangentToCircleTheoremFinderSettings LineTangentToCircleTheoremFinderSettings { get; }

        /// <summary>
        /// The types of theorems that we're looking for.
        /// </summary>
        public IEnumerable<TheoremType> SoughtTheoremTypes { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AlgorithmSettings"/> class.
        /// </summary>
        /// <param name="picturesSettings">The settings for the <see cref="Pictures"/> used in the algorithm.</param>
        /// <param name="tangentCirclesTheoremFinderSettings">The settings for the tangent circles theorem finder.</param>
        /// <param name="lineTangentToCircleTheoremFinderSettings">The settings for the line tangent to circle theorem finder.</param>
        /// <param name="soughtTheoremTypes">The types of theorems that we're looking for.</param>
        public AlgorithmSettings(PicturesSettings picturesSettings,
                                 TangentCirclesTheoremFinderSettings tangentCirclesTheoremFinderSettings,
                                 LineTangentToCircleTheoremFinderSettings lineTangentToCircleTheoremFinderSettings,
                                 IEnumerable<TheoremType> soughtTheoremTypes)
        {
            PicturesSettings = picturesSettings ?? throw new ArgumentNullException(nameof(picturesSettings));
            TangentCirclesTheoremFinderSettings = tangentCirclesTheoremFinderSettings ?? throw new ArgumentNullException(nameof(tangentCirclesTheoremFinderSettings));
            LineTangentToCircleTheoremFinderSettings = lineTangentToCircleTheoremFinderSettings ?? throw new ArgumentNullException(nameof(lineTangentToCircleTheoremFinderSettings));
            SoughtTheoremTypes = soughtTheoremTypes ?? throw new ArgumentNullException(nameof(soughtTheoremTypes));
        }

        #endregion
    }
}
