using System.Collections.Generic;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IObjectsConstructor"/> for <see cref="PredefinedConstructionType.IntersectionOfLines"/>>.
    /// </summary>
    public class IntersectionOfLinesConstructor : PredefinedConstructorBase
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
            // Pull passed lines
            var line1 = container.Get<Line>(flattenedObjects[0]);
            var line2 = container.Get<Line>(flattenedObjects[1]);

            // Try to make an intersection
            var intersection = line1.IntersectionWith(line2);

            // If it's null, return null; otherwise return the wrapped intersection
            return intersection == null ? null : new List<AnalyticObject> { intersection };
        }
    }
}