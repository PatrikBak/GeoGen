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

        private readonly IObjectsContainersManager _manager;

        private readonly ISubsetsProvider _provider;

        public ConcurrencyVerifier(IAnalyticalHelper helper, IObjectsContainersManager manager, ISubsetsProvider provider)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
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

            // Prepare dictionary mapping points to the objects that they lie on
            var dictionary = new Dictionary<Point, HashSet<GeometricalObject>>();

            // Pull first container
            var firstContainer = _manager.First();

            // Iterate over lines and circles to get all unordered pairs
            for (var i = 0; i < linesAndCircles.Count; i++)
            {
                for (var j = i + 1; j < linesAndCircles.Count; j++)
                {
                    // Pull their analytical version
                    var analytical1 = container.GetAnalyticalObject(linesAndCircles[i], firstContainer);
                    var analytical2 = container.GetAnalyticalObject(linesAndCircles[j], firstContainer);

                    // Intersect them
                    var intersections = _helper.Intersect(new List<AnalyticalObject> {analytical1, analytical2});

                    // We pick the intersections that are not presents in the configuration
                    var newIntesections = intersections.Where(intersection => !IsPointInContainer(container, firstContainer, intersection));

                    // Add register all of them to the dictionary
                    foreach (var newIntesection in newIntesections)
                    {
                        // Pull the set of passing objects
                        var passingObjects = dictionary.GetOrAdd(newIntesection, () => new HashSet<GeometricalObject>());

                        // Add our lines/circles to it
                        passingObjects.Add(linesAndCircles[i]);
                        passingObjects.Add(linesAndCircles[j]);
                    }
                }
            }

            // After constructing the intersections for one container, we may want to have a look at 
            // the ones that have at least 3 passing objects and construct verifier outputs for them
            foreach (var geometricalObjects in dictionary.Values.Where(objects => objects.Count >= 3))
            {
                // For these intersection we look at all 3-elements subsets (there are not gonna be many of them)
                foreach (var subset in _provider.GetSubsets(geometricalObjects.ToList(), 3))
                {
                    // We enumerate the subset
                    var involvedObjects = subset.ToList();

                    // Construct the verifier function
                    bool Verify(IObjectsContainer objectsContainer)
                    {
                        // If the container is the first one, we're sure the objects are fine
                        if (ReferenceEquals(firstContainer, objectsContainer))
                            return true;

                        // Otherwise we find analytical versions of our objects
                        var analyticalObjects = involvedObjects.Select(obj => container.GetAnalyticalObject(obj, objectsContainer));

                        // Intersect them
                        var newIntersections = _helper.Intersect(analyticalObjects);

                        // The output is correct if there is an intersection that is not in the configuration
                        return newIntersections.Any(intersection => !IsPointInContainer(container, objectsContainer, intersection));
                    }

                    // Construct the output
                    yield return new VerifierOutput
                    {
                        Type = TheoremType.ConcurrentObjects,
                        InvoldedObjects = involvedObjects,
                        AlwaysTrue = false,
                        VerifierFunction = Verify
                    };
                }
            }
        }

        /// <summary>
        /// Finds out if a given point is presents in the contextual container. 
        /// </summary>
        /// <param name="contextualContainer">The contextual container.</param>
        /// <param name="objectsContainer">The objects container for finding the configuration version of the point.</param>
        /// <param name="point">The point.</param>
        /// <returns>true, if the intersection is one of the contextual container's points; false otherwise.</returns>
        private bool IsPointInContainer(IContextualContainer contextualContainer, IObjectsContainer objectsContainer, Point point)
        {
            // For a given point, find the configuration object version in the container
            var configurationObject = objectsContainer.Get(point);

            // It's present if it's not null and the contextual container actually contains it
            return configurationObject != null && contextualContainer.Contains(configurationObject);
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