using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// A <see cref="ITheoremsFinder"/> for <see cref="TheoremType.ConcurrentObjects"/>.
    /// </summary>
    public class ConcurrentObjectsTheoremsFinder : IntersectingTheoremsFinder
    {
        #region Protected overridden properties

        /// <summary>
        /// Indicates whether we want intersected objects to have at least one 
        /// intersection that lies outside of the picture.
        /// </summary>
        protected override bool ExpectAnyExternalIntersection => true;

        #endregion

        #region Protected overridden methods

        /// <summary>
        /// Gets all options for a theorem represented as an array of geometric points.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <returns>An enumerable of all the options.</returns>
        protected override IEnumerable<GeometricObject[]> GetAllOptions(ContextualPicture contextualPicture)
        {
            // Simply take every triple of lines / circles to test for the intersections
            return contextualPicture.AllLinesAndCircles.Subsets(3);
        }

        /// <summary>
        /// Gets all options for a new theorem represented as an array of geometric points.
        /// Such theorems cannot be stated without the last object of the configuration. 
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <returns>An enumerable of all the options.</returns>
        protected override IEnumerable<GeometricObject[]> GetNewOptions(ContextualPicture contextualPicture)
        {
            // Find new lines / circles
            var newLinesCircles = contextualPicture.NewLinesAndCircles.ToList();

            // Find old lines / circles
            var oldLinesCircles = contextualPicture.OldLinesAndCircles.ToList();

            // Combine three new objects
            foreach (var (newLineCircle1, newLineCircle2, newLineCircle3) in newLinesCircles.UnorderedTriples())
                yield return new[] { newLineCircle1, newLineCircle2, newLineCircle3 };

            // Combine two new objects and one old
            foreach (var (newLineCircle1, newLineCircle2) in newLinesCircles.UnorderedPairs())
                foreach (var oldLineCircle in oldLinesCircles)
                    yield return new[] { newLineCircle1, newLineCircle2, oldLineCircle };

            // Combine one new object and two old
            foreach (var newLineCircle in newLinesCircles)
                foreach (var (oldLineCircle1, oldLineCircle2) in oldLinesCircles.UnorderedPairs())
                    yield return new[] { newLineCircle, oldLineCircle1, oldLineCircle2 };
        }

        /// <summary>
        /// Returns if a given number intersections is allowed for this type of theorem.
        /// </summary>
        /// <param name="numberOfIntersections">The number of intersections to be questioned.</param>
        /// <returns>true, if this number is allowed; false otherwise.</returns>
        protected override bool IsNumberOfIntersectionsAllowed(int numberOfIntersections) => numberOfIntersections >= 1;

        #endregion
    }
}