using GeoGen.Core;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// The settings related to the theorem finder module.
    /// </summary>
    public class TheoremFindingSettings
    {
        #region Public properties

        /// <summary>
        /// The types of theorems that we're looking for.
        /// </summary>
        public IEnumerable<TheoremType> SoughtTheoremTypes { get; }

        /// <summary>
        /// The settings for <see cref="TangentCirclesTheoremFinder"/>. It can be null if this type is not sought.
        /// </summary>
        public TangentCirclesTheoremFinderSettings TangentCirclesTheoremFinderSettings { get; }

        /// <summary>
        /// The settings for <see cref="LineTangentToCircleTheoremFinder"/>. It can be null if this type is not sought.
        /// </summary>
        public LineTangentToCircleTheoremFinderSettings LineTangentToCircleTheoremFinderSettings { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremFindingSettings"/> class.
        /// </summary>
        /// <param name="soughtTheoremTypes">The types of theorems that we're looking for.</param>
        /// <param name="tangentCirclesTheoremFinderSettings">he settings for <see cref="TangentCirclesTheoremFinder"/>. It can be null if this type is not sought.</param>
        /// <param name="lineTangentToCircleTheoremFinderSettings">The settings for <see cref="LineTangentToCircleTheoremFinder"/>. It can be null if this type is not sought.</param>
        public TheoremFindingSettings(IReadOnlyCollection<TheoremType> soughtTheoremTypes,
                                      TangentCirclesTheoremFinderSettings tangentCirclesTheoremFinderSettings,
                                      LineTangentToCircleTheoremFinderSettings lineTangentToCircleTheoremFinderSettings)
        {
            SoughtTheoremTypes = soughtTheoremTypes ?? throw new ArgumentNullException(nameof(soughtTheoremTypes));
            TangentCirclesTheoremFinderSettings = tangentCirclesTheoremFinderSettings;
            LineTangentToCircleTheoremFinderSettings = lineTangentToCircleTheoremFinderSettings;

            // Ensure that tangent circles theorem finder settings are set if this theorem type is sought
            if (soughtTheoremTypes.Contains(TheoremType.TangentCircles) && tangentCirclesTheoremFinderSettings == null)
                throw new TheoremFinderException("The tangent circles theorem finder must have its settings set as this theorem type is sought.");

            // Ensure that line tangent to circle theorem finder settings are set if this theorem type is sought
            if (soughtTheoremTypes.Contains(TheoremType.LineTangentToCircle) && lineTangentToCircleTheoremFinderSettings == null)
                throw new TheoremFinderException("The line tangent to circle theorem finder must have its settings set as this theorem type is sought.");
        }

        #endregion
    }
}
