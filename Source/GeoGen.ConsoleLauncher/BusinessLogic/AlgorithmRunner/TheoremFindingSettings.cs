using GeoGen.Core;
using GeoGen.TheoremFinder;
using System;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings related to the theorem finder module.
    /// </summary>
    public class TheoremFindingSettings
    {
        #region Public properties

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
        /// Initializes a new instance of the <see cref="TheoremFindingSettings"/> class.
        /// </summary>
        /// <param name="tangentCirclesTheoremFinderSettings">The settings for the tangent circles theorem finder.</param>
        /// <param name="lineTangentToCircleTheoremFinderSettings">The settings for the line tangent to circle theorem finder.</param>
        /// <param name="soughtTheoremTypes">The types of theorems that we're looking for.</param>
        public TheoremFindingSettings(TangentCirclesTheoremFinderSettings tangentCirclesTheoremFinderSettings,
                                     LineTangentToCircleTheoremFinderSettings lineTangentToCircleTheoremFinderSettings,
                                     IEnumerable<TheoremType> soughtTheoremTypes)
        {
            TangentCirclesTheoremFinderSettings = tangentCirclesTheoremFinderSettings ?? throw new ArgumentNullException(nameof(tangentCirclesTheoremFinderSettings));
            LineTangentToCircleTheoremFinderSettings = lineTangentToCircleTheoremFinderSettings ?? throw new ArgumentNullException(nameof(lineTangentToCircleTheoremFinderSettings));
            SoughtTheoremTypes = soughtTheoremTypes ?? throw new ArgumentNullException(nameof(soughtTheoremTypes));
        }

        #endregion
    }
}
