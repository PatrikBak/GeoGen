using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.IntersectionOfLinesFromLineAndPoints"/>>.
    /// </summary>
    internal class IntersectionOfLinesFromLineAndPointsConstructor : PredefinedConstructorBase
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
            // Pull passed line
            var line = container.Get<Line>(flattenedObjects[0]);

            // And passed points
            var point1 = container.Get<Point>(flattenedObjects[1]);
            var point2 = container.Get<Point>(flattenedObjects[2]);

            try
            {
                // Try to make an intersection
                var intersection = line.IntersectionWith(new Line(point1, point2));

                // If it doesn't exist, return null; otherwise return the wrapped intersection
                return intersection == null ? null : new List<AnalyticalObject> {intersection};
            }
            catch (AnalyticalException)
            {
                // If the lines are equal, return null
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