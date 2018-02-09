using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.CircumcenterFromPoints"/>>.
    /// </summary>
    internal class CircumcenterFromPointsConstructor : PredefinedConstructorBase
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
            // Pull points from the container
            var point1 = container.Get<Point>(flattenedObjects[0]);
            var point2 = container.Get<Point>(flattenedObjects[1]);
            var point3 = container.Get<Point>(flattenedObjects[2]);

            try
            {
                // Try to construct the circle
                return new List<AnalyticalObject> {new Circle(point1, point2, point3).Center};
            }
            catch (AnalyticalException)
            {
                // If it can't be done (points are equal / collinear), return null
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
            return new List<Theorem>();
        }
    }
}