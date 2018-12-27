using System.Collections.Generic;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.IntersectionOfLinesFromPoints"/>>.
    /// </summary>
    public class IntersectionOfLinesFromPointsConstructor : PredefinedConstructorBase
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
            // Pull points
            var point1 = container.Get<Point>(flattenedObjects[0]);
            var point2 = container.Get<Point>(flattenedObjects[1]);
            var point3 = container.Get<Point>(flattenedObjects[2]);
            var point4 = container.Get<Point>(flattenedObjects[3]);

            // Create lines. 
            var line1 = new Line(point1, point2);
            var line2 = new Line(point3, point4);

            // Check if lines are equal
            if (line1 == line2)
                return null;

            // Intersect them
            var intersection = line1.IntersectionWith(line2);

            // If there is no intersection, return null
            if (intersection == null)
                return null;

            // Otherwise return in wrapped in the list
            return new List<AnalyticObject> { intersection };
        }
    }
}