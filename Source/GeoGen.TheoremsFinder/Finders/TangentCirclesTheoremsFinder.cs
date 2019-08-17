using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// A <see cref="ITheoremsFinder"/> for <see cref="TheoremType.TangentCircles"/>.
    /// </summary>
    public class TangentCirclesTheoremsFinder : TrueInAllPicturesTheoremsFinder
    {
        /// <summary>
        /// Gets all options for a theorem represented as an array of geometric points.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <returns>An enumerable of all the options.</returns>
        protected override IEnumerable<GeometricObject[]> GetAllOptions(ContextualPicture contextualPicture)
        {
            // Get all pairs of circles
            return contextualPicture.AllCircles.Subsets(2);
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

            // Combine the new circles with themselves
            foreach (var (newCircle1, newCircle2) in newCircles.UnorderedPairs())
                yield return new[] { newCircle1, newCircle2 };

            // Combine the new circles with the old ones
            foreach (var newCircle in newCircles)
                foreach (var oldCircle in oldCircles)
                    yield return new[] { newCircle, oldCircle };
        }

        /// <summary>
        /// Finds out if the theorem given in analytic objects holds true.
        /// </summary>
        /// <param name="objects">The analytic objects.</param>
        /// <returns>true, if the theorem holds true; false otherwise.</returns>
        protected override bool IsTrue(IAnalyticObject[] objects)
        {
            // Return if they are tangent to each other
            return ((Circle)objects[0]).IsTangentTo((Circle)objects[1]);
        }
    }
}
