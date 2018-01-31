using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="ITheoremVerifier"/> for <see cref="TheoremType.ConcurrentObjects"/>.
    /// </summary>
    internal class ConcurrencyVerifier : ITheoremVerifier
    {
        private readonly IAnalyticalHelper _helper;

        private readonly ISubsetsProvider _provider;

        public ConcurrencyVerifier(IAnalyticalHelper helper, ISubsetsProvider provider)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        /// Gets the enumerable of verifier outputs that pulls objects from
        /// a given contextual container (that represents the configuration)
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The outputs.</returns>
        public IEnumerable<VerifierOutput> GetOutput(IContextualContainer container)
        {
            // Merge all lines and circles
            var linesAndCircles = container.GetGeometricalObjects<LineObject>()
                    .Cast<GeometricalObject>()
                    .Concat(container.GetGeometricalObjects<CircleObject>())
                    .ToList();

            foreach (var objects in _provider.GetSubsets(linesAndCircles, 3))
            {
                // Enumerate involved objects
                var involdedObjects = objects.ToList();

                // Verify if the intersection can't be determined without
                // constructing actual intersections
                // if (!IsWorthAnalalyzing(involdedObjects)) continue;

                // Declare the verifier function
                bool Verify(IObjectsContainer objectsContainer)
                {
                    // Find analytical versions of our objects
                    var analyticalObjects = involdedObjects.Select(obj => container.GetAnalyticalObject(obj, objectsContainer));

                    // Intersect them
                    var newIntersections = _helper.Intersect(analyticalObjects);

                    // If there is no intersection, we're done
                    if (newIntersections.Empty())
                        return false;

                    // Otherwise the output is correct if there is an intersection
                    // that is not in the configuration
                    return newIntersections.Any(intersection =>
                    {
                        // For a given point, find the configuration object version in the container
                        var configurationObject = objectsContainer.Get(intersection);

                        // If fine if it doesn't exist (it's null) or if it's not in our configuration
                        // (i.e. in the contextual container)
                        return configurationObject == null || !container.Contains(configurationObject);
                    });
                }

                // Construct the output
                yield return new VerifierOutput
                {
                    Type = TheoremType.ConcurrentObjects,
                    InvoldedObjects = involdedObjects,
                    AlwaysTrue = false,
                    VerifierFunction = Verify
                };
            }
        }

        /// <summary>
        /// Finds out if the intersection(s) of given objects couldn't be found
        /// in the points that they pass through.
        /// </summary>
        /// <param name="involvedObjects">The involved objects.</param>
        /// <returns>true, </returns>
        private bool IsWorthAnalalyzing(List<GeometricalObject> involvedObjects)
        {
            // Prepare set holding the common points
            HashSet<PointObject> commonPoints = null;

            // Prepare variable indicating the number of lines that we're intersection
            var numberOfLines = 0;

            // Iterate over objects
            foreach (var geometricalObject in involvedObjects)
            {
                // If we have a line, mark it
                if (geometricalObject is LineObject)
                    numberOfLines++;

                // We're sure the objects holds points...
                var currentPoints = ((DefinableByPoints) geometricalObject).Points;

                // If our points are not set
                if (commonPoints == null)
                {
                    // Construct them
                    commonPoints = new HashSet<PointObject>(currentPoints);

                    // And continue
                    continue;
                }

                // Otherwise remove the ones that are not between the current ones
                commonPoints.RemoveWhere(currentPoints.Contains);

                // If we have no common points, we can terminate directly
                return true;
            }

            // If there is no common point, then finding the intersections is worth testing
            if (commonPoints.Empty())
                return true;

            // If there is at least one of them  and we are intersecting 
            // at least 2 lines, than we know the intersection
            if (numberOfLines >= 2)
                return false;

            // Otherwise we might have two intersections. The intersections are worth testing
            // if we don't have two of them
            return commonPoints.Count != 2;
        }
    }
}