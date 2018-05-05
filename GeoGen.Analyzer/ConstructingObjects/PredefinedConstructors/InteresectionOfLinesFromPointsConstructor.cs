using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.IntersectionOfLinesFromPoints"/>>.
    /// </summary>
    internal class IntersectionOfLinesFromPointsConstructor : PredefinedConstructorBase
    {
        /// <summary>
        /// Constructs a list of analytical objects from a given list of 
        /// flattened objects from the arguments and a container that is used to 
        /// obtain the actual analytical versions of these objects.
        /// </summary>
        /// <param name="flattenedObjects">The flattened argument objects.</param>
        /// <param name="container">The objects container.</param>
        /// <returns>The list of constructed analytical objects.</returns>
        protected override List<AnalyticalObject> Construct(IReadOnlyList<ConfigurationObject> flattenedObjects, IObjectsContainer container)
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
            return new List<AnalyticalObject> {intersection};
        }

        /// <summary>
        /// Constructs a list of default theorems using a newly constructed objects and
        /// flattened objects from the passed arguments.
        /// </summary>
        /// <param name="input">The constructed objects.</param>
        /// <param name="flattenedObjects">The flattened argument objects.</param>
        /// <returns>The list of default theorems.</returns>
        protected override List<Theorem> FindDefaultTheorms(IReadOnlyList<ConstructedConfigurationObject> input, IReadOnlyList<ConfigurationObject> flattenedObjects)
        {
            return new List<Theorem>
            {
                // The intersection is collinear with first two points
                new Theorem(TheoremType.CollinearPoints, new List<TheoremObject>
                {
                    new TheoremObject(flattenedObjects[0]),
                    new TheoremObject(flattenedObjects[1]),
                    new TheoremObject(input[0])
                }),
                // As well as with the other two points
                new Theorem(TheoremType.CollinearPoints, new List<TheoremObject>
                {
                    new TheoremObject(flattenedObjects[2]),
                    new TheoremObject(flattenedObjects[3]),
                    new TheoremObject(input[0])
                })
            };
        }
    }
}