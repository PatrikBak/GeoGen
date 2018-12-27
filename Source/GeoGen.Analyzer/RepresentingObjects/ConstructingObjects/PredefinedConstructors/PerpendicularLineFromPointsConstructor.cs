using System.Collections.Generic;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.PerpendicularLineFromPoints"/>>.
    /// </summary>
    public class PerpendicularLineFromPointsConstructor : PredefinedConstructorBase
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
            // Pull line points
            var linePoint1 = container.Get<Point>(flattenedObjects[1]);
            var linePoint2 = container.Get<Point>(flattenedObjects[2]);

            // Pull the point from which we erect the perpendicular like
            var pointFrom = container.Get<Point>(flattenedObjects[0]);

            // Construct the line
            var line = new Line(linePoint1, linePoint2);

            // Construct the result
            return new List<AnalyticObject> { line.PerpendicularLine(pointFrom) };
        }
    }
}