using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// A <see cref="ITypedTheoremFinder"/> for <see cref="TheoremType.ConcurrentObjects"/>.
    /// </summary>
    public class ConcurrentObjectsTheoremFinder : IntersectingTheoremFinder
    {
        #region Protected overridden properties

        /// <inheritdoc/>
        protected override bool ExpectAnyExternalIntersection => true;

        #endregion

        #region Protected overridden methods

        /// <inheritdoc/>
        protected override IEnumerable<GeometricObject[]> GetAllOptions(ContextualPicture contextualPicture)
            // Simply take every triple of lines / circles to test for the intersections
            => contextualPicture.AllLinesAndCircles.Subsets(3);

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        protected override bool IsNumberOfIntersectionsAllowed(int numberOfIntersections) => numberOfIntersections >= 1;

        #endregion
    }
}