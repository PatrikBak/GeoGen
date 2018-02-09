using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.InternalAngleBisectorFromPoints"/>>.
    /// </summary>
    internal class InternalAngleBisectorFromPointsConstructor : PredefinedConstructorBase
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
            // Pull points on ray
            var point1 = container.Get<Point>(flattenedObjects[1]);
            var point2 = container.Get<Point>(flattenedObjects[2]);

            // Pull the rays intersection
            var intersection = container.Get<Point>(flattenedObjects[0]);

            try
            {
                // Try to create the internal bisector
                return new List<AnalyticalObject> {intersection.InternalAngleBisector(point1, point2)};
            }
            catch (AnalyticalException)
            {
                // If it's not successful, return null
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