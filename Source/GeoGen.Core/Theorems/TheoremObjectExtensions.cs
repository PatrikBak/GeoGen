using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Extensions methods for <see cref="TheoremObject"/>
    /// </summary>
    public static class TheoremObjectExtensions
    {
        /// <summary>
        /// Enumerates every possible set of objects that are altogether needed to define this object (this includes even 
        /// defining objects of objects, see <see cref="ConfigurationObjectsExtentions.GetDefiningObjects(ConfigurationObject)"/>.
        /// For example: If we have a line 'l' with points A, B, C on it, then this line has 4 possible definitions: 
        /// l, [A, B], [A, C], [B, C]. 
        /// </summary>
        /// <param name="theoremObject">The theorem object</param>
        /// <returns>The enumerable of objects representing a definition.</returns>
        public static IEnumerable<IEnumerable<ConfigurationObject>> GetAllDefinitions(this TheoremObject theoremObject)
        {
            // Switch on the type
            switch (theoremObject)
            {
                // Base objects might have an object part
                case BaseTheoremObject baseObject:

                    // If there is an object part, then its definition is one result
                    if (baseObject.ConfigurationObject != null)
                        yield return baseObject.ConfigurationObject.GetDefiningObjects();

                    // Switch further on type of base object
                    switch (baseObject)
                    {
                        // If we have a point, we have no more definitions
                        case PointTheoremObject point:
                            yield break;

                        // If we have an object with points...
                        case TheoremObjectWithPoints objectWithPoints:

                            // For every possible n-tuple of points 
                            foreach (var points in objectWithPoints.Points.Subsets(objectWithPoints.NumberOfNeededPoints))
                            {
                                // Consider its defining objects as a potential definition
                                yield return points.GetDefiningObjects();
                            }

                            yield break;

                        // Default case
                        default:
                            throw new GeoGenException($"Unhandled type of base theorem object: {baseObject.GetType()}");
                    }

                // Objects with two theorem objects
                case PairTheoremObject pairObject:

                    // Get the definitions of particular objects
                    var definitions = new[]
                    {
                        pairObject.Object1.GetAllDefinitions(),
                        pairObject.Object2.GetAllDefinitions()
                    }
                    // Combine them
                    .Combine()
                    // Take definitions of these objects
                    .Select(definition => definition.Flatten().Distinct());

                    // Return these definitions
                    foreach (var definition in definitions)
                        yield return definition;

                    yield break;

                // Default case
                default:
                    throw new GeoGenException($"Unhandled type of theorem object: {theoremObject.GetType()}");
            }
        }
    }
}
