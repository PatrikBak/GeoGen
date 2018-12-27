using System;
using System.Collections.Generic;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.CircumcenterFromPoints"/>>.
    /// </summary>
    public class CircumcenterFromPointsConstructor : PredefinedConstructorBase
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
            // Pull points from the container
            var point1 = container.Get<Point>(flattenedObjects[0]);
            var point2 = container.Get<Point>(flattenedObjects[1]);
            var point3 = container.Get<Point>(flattenedObjects[2]);

            // If points are collinear, the construction can't be done
            if (AnalyticHelpers.AreCollinear(point1, point2, point3))
                return null;

            // Otherwise construct the circle and take its center
            return new List<AnalyticObject> { new Circle(point1, point2, point3).Center };
        }
    }
}