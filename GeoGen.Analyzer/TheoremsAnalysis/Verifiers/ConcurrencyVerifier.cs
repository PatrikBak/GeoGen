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
        private class SetHolder
        {
            public HashSet<GeometricalObject> Objects { get; }

            public HashSet<IObjectsContainer> Containers { get; }

            public SetHolder(IEnumerable<GeometricalObject> objects)
            {
                Objects = new HashSet<GeometricalObject>(objects);
                Containers = new HashSet<IObjectsContainer>();
            }

            public override int GetHashCode()
            {
                return SetComparer<GeometricalObject>.Instance.GetHashCode(Objects);
            }

            public override bool Equals(object obj)
            {
                return SetComparer<GeometricalObject>.Instance.Equals(Objects, ((SetHolder) obj).Objects);
            }
        }

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

            // Prepare dictionary mapping points to the objects that they lie on for all containers
            var dictionary = new Dictionary<IObjectsContainer, Dictionary<Point, HashSet<GeometricalObject>>>();

            // For all containers 
            foreach (var objectsContainer in _manager)
            {
                // Prepare new dictionary for this container
                var intersectionToObjects = new Dictionary<Point, HashSet<GeometricalObject>>();

                // Add it to the main dictionary
                dictionary.Add(objectsContainer, intersectionToObjects);

                // Iterate over lines and circles to get all unordered pairs
                for (var i = 0; i < linesAndCircles.Count; i++)
                {
                    for (var j = i + 1; j < linesAndCircles.Count; j++)
                    {
                        // Pull their analytical version
                        var analytical1 = container.GetAnalyticalObject(linesAndCircles[i], objectsContainer);
                        var analytical2 = container.GetAnalyticalObject(linesAndCircles[j], objectsContainer);

                        // Intersect them
                        var intersections = _helper.Intersect(new List<AnalyticalObject> {analytical1, analytical2})
                                // And take those that aren't in our configuration
                                .Where(intersection => !IsPointInContainer(container, objectsContainer, intersection));

                        // Register all these intersections to the dictionary
                        foreach (var newIntesection in intersections)
                        {
                            // Pull the set of passing objects
                            var passingObjects = intersectionToObjects.GetOrAdd(newIntesection, () => new HashSet<GeometricalObject>());

                            // Add our lines/circles to it
                            passingObjects.Add(linesAndCircles[i]);
                            passingObjects.Add(linesAndCircles[j]);
                        }
                    }
                }
            }

            // Prepare the dictionary that holds our set holders. In this dictionary,
            // the instances are mapped to the equal ones so we can have a function AddOrGet,
            // which a normal HashSet doesn't provide 
            var setHolders = new Dictionary<SetHolder, SetHolder>();

            // Iterate over all pairs container - intersections
            foreach (var pair in dictionary)
            {
                // Pull container
                var currentContainer = pair.Key;

                // Pull sets of intersections
                var setsOfIntersections = pair.Value.Values;

                // Go through all of them
                foreach (var objects in setsOfIntersections)
                {
                    // Skip the ones with fewer than 3 elements
                    if (objects.Count < 3)
                        continue;

                    // Create all triples for the other ones
                    foreach (var triple in _provider.GetSubsets(objects.ToList(), 3))
                    {
                        // Create a set holder for it
                        var setHolder = new SetHolder(triple);

                        // Add or get it from the dictionary
                        var holderFromDictionary = setHolders.GetOrAdd(setHolder, () => setHolder);

                        // Mark the container
                        holderFromDictionary.Containers.Add(currentContainer);
                    }
                }

                // Now are all interesting things are calculated in the holders set
                foreach (var holder in setHolders.Keys)
                {
                    // We can easily construct the verifier function
                    bool Verify(IObjectsContainer objectsContainer)
                    {
                        // Because now the objects are correct if and only if
                        // the given container is registered to the holder
                        return holder.Containers.Contains(objectsContainer);
                    }

                    // And construct the output
                    yield return new VerifierOutput
                    {
                        Type = TheoremType.ConcurrentObjects,
                        InvoldedObjects = holder.Objects,
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