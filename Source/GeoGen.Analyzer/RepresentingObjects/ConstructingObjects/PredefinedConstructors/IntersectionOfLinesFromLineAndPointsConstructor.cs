using System.Collections.Generic;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.IntersectionOfLinesFromLineAndPoints"/>>.
    /// </summary>
    public class IntersectionOfLinesFromLineAndPointsConstructor : PredefinedConstructorBase
    {
        /// <summary>
        /// Constructs a list of analytic objects from a given list of 
        /// flattened objects from the arguments and a container that is used to 
        /// obtain the actual analytic versions of these objects.
        /// </summary>
        /// <param name="flattenedObjects">The flattened argument objects.</param>
        /// <param name="container">The objects container.</param>
        /// <returns>The list of constructed analytic objects.</returns>
        protected override List<AnalyticObject> Construct(IReadOnlyList<ConfigurationObject> flattenedObjects, IObjectsContainer container)
        {
            // Pull passed line
            var line = container.Get<Line>(flattenedObjects[0]);

            // And passed points
            var point1 = container.Get<Point>(flattenedObjects[1]);
            var point2 = container.Get<Point>(flattenedObjects[2]);

            // Create the other line
            var otherLine = new Line(point1, point2);

            // If the lines are the same
            if (line == otherLine)
            {
                // Then the construction is not possible
                return null;
            }

            // Otherwise we make their intersection
            var intersection = line.IntersectionWith(otherLine);

            // If it doesn't exist, return null; otherwise return the wrapped intersection
            return intersection == null ? null : new List<AnalyticObject> { intersection };
        }
    }
}