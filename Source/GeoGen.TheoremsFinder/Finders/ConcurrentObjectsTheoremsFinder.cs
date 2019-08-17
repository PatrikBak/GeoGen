using GeoGen.AnalyticGeometry;
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
    public class ConcurrentObjectsTheoremsFinder : AbstractTheoremsFinder
    {
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
        /// Finds out if given geometric objects represent a true theorem.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that stores the geometric points.</param>
        /// <param name="objects">The geometric objects that represent the theorem.</param>
        /// <returns>true, if the theorem holds true; false otherwise.</returns>
        protected override bool RepresentsTrueTheorem(ContextualPicture contextualPicture, GeometricObject[] objects)
        {
            // Prepare the variable that indicates whether the objects
            // have an intersection that lies outside of some picture
            var anyExternalIntersection = false;

            // We want these objects to have an intersection that 
            // is not in the picture for every single picture
            foreach (var picture in contextualPicture.Pictures)
            {
                // For a given picture we take the objects
                var analyticObject1 = contextualPicture.GetAnalyticObject(objects[0], picture);
                var analyticObject2 = contextualPicture.GetAnalyticObject(objects[1], picture);
                var analyticObject3 = contextualPicture.GetAnalyticObject(objects[2], picture);

                // Intersect them
                var intersections = AnalyticHelpers.Intersect(analyticObject1, analyticObject2, analyticObject3);

                // If there is no intersection, then this won't be a theorem
                if (intersections.Length == 0)
                    return false;

                // Mark if there is any intersection that is not in the picture
                if (intersections.Any(point => !picture.Contains(point)))
                    anyExternalIntersection = true;
            }

            // At this point, the theorem is interesting if and only if
            // there is an external intersection in some picture (i.e.
            // it is not true that in every picture all the intersections
            // lie inside the picture)
            return anyExternalIntersection;
        }
    }
}