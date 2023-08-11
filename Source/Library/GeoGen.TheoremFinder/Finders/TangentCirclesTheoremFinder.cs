using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// A <see cref="ITypedTheoremFinder"/> for <see cref="TheoremType.TangentCircles"/>.
    /// </summary>
    public class TangentCirclesTheoremFinder : IntersectingTheoremFinder
    {
        #region Private fields

        /// <summary>
        /// The settings for the finder.
        /// </summary>
        private readonly TangentCirclesTheoremFinderSettings _settings;

        #endregion

        #region Protected overridden properties

        /// <inheritdoc/>
        protected override bool ExpectAnyExternalIntersection => _settings.ExcludeTangencyInsidePicture;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TangentCirclesTheoremFinder"/> class.
        /// </summary>
        /// <param name="settings">The settings for the finder.</param>
        public TangentCirclesTheoremFinder(TangentCirclesTheoremFinderSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region Protected overridden methods

        /// <inheritdoc/>
        protected override IEnumerable<GeometricObject[]> GetAllOptions(ContextualPicture contextualPicture)
        {
            // If we are excluding tangencies inside picture, we 
            // don't have to consider circles with a common point
            if (_settings.ExcludeTangencyInsidePicture)
            {
                // Combine all circles with themselves
                foreach (var circles in contextualPicture.AllCircles.Subsets(2))
                    if (circles[0].CommonPointsWith(circles[1]).IsEmpty())
                        yield return circles;
            }
            // Otherwise we don't have to consider circles with 2 common points
            else
            {
                // Combine all circles with themselves
                foreach (var circles in contextualPicture.AllCircles.Subsets(2))
                    if (circles[0].CommonPointsWith(circles[1]).Count() != 2)
                        yield return circles;
            }
        }

        /// <inheritdoc/>
        protected override IEnumerable<GeometricObject[]> GetNewOptions(ContextualPicture contextualPicture)
        {
            // Find new circles. 
            var newCircles = contextualPicture.NewCircles.ToList();

            // Find old circles.
            var oldCircles = contextualPicture.OldCircles.ToList();

            // If we are excluding tangencies inside picture, we 
            // don't have to consider circles with a common point
            if (_settings.ExcludeTangencyInsidePicture)
            {
                // Combine the new circles with themselves
                foreach (var circles in newCircles.Subsets(2))
                    if (circles[0].CommonPointsWith(circles[1]).IsEmpty())
                        yield return circles;

                // Combine the new circles with the old ones
                foreach (var newCircle in newCircles)
                    foreach (var oldCircle in oldCircles)
                        if (newCircle.CommonPointsWith(oldCircle).IsEmpty())
                            yield return new[] { newCircle, oldCircle };
            }
            // Otherwise we don't have to consider circles with 2 common points
            else
            {
                // Combine the new circles with themselves
                foreach (var circles in newCircles.Subsets(2))
                    if (circles[0].CommonPointsWith(circles[1]).Count() != 2)
                        yield return circles;

                // Combine the new circles with the old ones
                foreach (var newCircle in newCircles)
                    foreach (var oldCircle in oldCircles)
                        if (newCircle.CommonPointsWith(oldCircle).Count() != 2)
                            yield return new[] { newCircle, oldCircle };
            }
        }

        #endregion
    }
}
