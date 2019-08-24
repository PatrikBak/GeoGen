using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// A <see cref="ITheoremsFinder"/> for <see cref="TheoremType.TangentCircles"/>.
    /// </summary>
    public class TangentCirclesTheoremsFinder : IntersectingTheoremsFinder
    {
        #region Private fields

        /// <summary>
        /// The settings for the finder.
        /// </summary>
        private readonly TangentCirclesTheoremsFinderSettings _settings;

        #endregion

        #region Protected overridden properties

        /// <summary>
        /// Indicates whether we want intersected objects to have at least one 
        /// intersection that lies outside of the picture.
        /// </summary>
        protected override bool ExpectAnyExternalIntersection => _settings.ExcludeTangencyInsidePicture;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TangentCirclesTheoremsFinder"/> class.
        /// </summary>
        /// <param name="settings">The settings for the finder.</param>
        public TangentCirclesTheoremsFinder(TangentCirclesTheoremsFinderSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region Protected overridden methods

        /// <summary>
        /// Gets all options for a theorem represented as an array of geometric points.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <returns>An enumerable of all the options.</returns>
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

        /// <summary>
        /// Gets all options for a new theorem represented as an array of geometric points.
        /// Such theorems cannot be stated without the last object of the configuration. 
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <returns>An enumerable of all the options.</returns>
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

        /// <summary>
        /// Returns if a given number intersections is allowed for this type of theorem.
        /// </summary>
        /// <param name="numberOfIntersections">The number of intersections to be questioned.</param>
        /// <returns>true, if this number is allowed; false otherwise.</returns> 
        protected override bool IsNumberOfIntersectionsAllowed(int numberOfIntersections) => numberOfIntersections == 1;

        #endregion
    }
}
