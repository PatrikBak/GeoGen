using System.Collections.Generic;

namespace GeoGen.AnalyticalGeometry
{
    /// <summary>
    /// Represents a helper service that simplifies and generalizes interactions
    /// with analytical objects.
    /// </summary>
    public interface IAnalyticalHelper
    {
        /// <summary>
        /// Finds all intersections of given analytical objects. They must not
        /// contain <see cref="Point"/>s and duplicate objects.
        /// </summary>
        /// <param name="inputObjects">The input objects.</param>
        /// <returns>The set of intersections. An empty set, if there's none.</returns>
        HashSet<Point> Intersect(IEnumerable<AnalyticalObject> inputObjects);

        /// <summary>
        /// Checks if a given point lies on a given analytical object. The object
        /// must not be a point.
        /// </summary>
        /// <param name="analyticalObject">The analytical object.</param>
        /// <param name="point">The point.</param>
        /// <returns>true, if the point lies on the object, false otherwise.</returns>
        bool LiesOn(AnalyticalObject analyticalObject, Point point);
    }
}