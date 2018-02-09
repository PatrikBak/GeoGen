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
        protected override List<AnalyticalObject> Construct(List<ConfigurationObject> flattenedObjects, IObjectsContainer container)
        {
            // Pull points
            var point1 = container.Get<Point>(flattenedObjects[0]);
            var point2 = container.Get<Point>(flattenedObjects[1]);
            var point3 = container.Get<Point>(flattenedObjects[2]);
            var point4 = container.Get<Point>(flattenedObjects[3]);

            try
            {
                // Create the set of our points
                var points = new HashSet<Point> {point1, point2, point3, point4};

                // Create lines. This might throw an AnalyticalException if the points are same 
                var line1 = new Line(point1, point2);
                var line2 = new Line(point3, point4);

                // If the lines are fine, intersect them
                var result = line1.IntersectionWith(line2);

                // If there is no intersection, or the intersection is the same as some 
                // of our points (which is not allowed because of the contract of this constructor)
                if (result == null || points.Contains(result))
                    return null;

                // Otherwise the point is correct, we can return it wrapped in a list
                return new List<AnalyticalObject> {result};
            }
            catch (AnalyticalException)
            {
                // If we got here, then we have either equal points, or equal lines
                // In that case, the construction has failed
                return null;
            }
        }

        /// <summary>
        /// Constructs a list of default theorems using a newly constructed objects and
        /// flattened objects from the passed arguments.
        /// </summary>
        /// <param name="input">The constructed objects.</param>
        /// <param name="flattenedObjects">The flattened argument objects.</param>
        /// <returns>The list of default theorems.</returns>
        protected override List<Theorem> FindDefaultTheorms(List<ConstructedConfigurationObject> input, List<ConfigurationObject> flattenedObjects)
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