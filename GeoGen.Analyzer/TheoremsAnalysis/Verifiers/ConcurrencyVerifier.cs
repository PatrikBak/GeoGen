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
        #region Private fields

        /// <summary>
        /// The helper of intersecting analytical objects.
        /// </summary>
        private readonly IAnalyticalHelper _helper;

        /// <summary>
        /// The manager of all objects containers.
        /// </summary>
        private readonly IObjectsContainersManager _manager;

        /// <summary>
        /// The generator of subsets of given length.
        /// </summary>
        private readonly ISubsetsProvider _provider;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="helper">The analytical helper.</param>
        /// <param name="manager">The manager for containers.</param>
        /// <param name="provider">The subsets generator.</param>
        public ConcurrencyVerifier(IAnalyticalHelper helper, IObjectsContainersManager manager, ISubsetsProvider provider)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        #endregion

        #region ITheoremVerifier implementation

        /// <summary>
        /// Gets the enumerable of verifier outputs that pulls objects from
        /// a given contextual container (that represents the configuration)
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The outputs.</returns>
        public IEnumerable<VerifierOutput> GetOutput(IContextualContainer container)
        {
            // Find all new lines / circles. At least one of them must be included
            // in a new theorem
            var newLinesCircles = container.GetGeometricalObjects<GeometricalObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludePoints = false,
                IncludeLines = true,
                IncludeCirces = true
            }).ToList();

            // Find all lines / circles. These will be combined with the new ones
            var allLinesCircles = container.GetGeometricalObjects<GeometricalObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.All,
                IncludePoints = false,
                IncludeLines = true,
                IncludeCirces = true
            }).ToList();

            // Prepare dictionary mapping points to the objects that they lie on
            var dictionary = new Dictionary<Point, HashSet<GeometricalObject>>();

            // Pull first container
            var firstContainer = _manager.First();

            // Iterate over lines and circles to get all pairs
            foreach (var lineOrCircle1 in newLinesCircles)
            {
                foreach (var lineOrCircle2 in allLinesCircles)
                {
                    // If they are equal, we skip them
                    if (Equals(lineOrCircle1, lineOrCircle2))
                        continue;

                    // Pull their analytical version
                    var analytical1 = container.GetAnalyticalObject(lineOrCircle1, firstContainer);
                    var analytical2 = container.GetAnalyticalObject(lineOrCircle2, firstContainer);

                    // Intersect them
                    var intersections = _helper.Intersect(new List<AnalyticalObject> { analytical1, analytical2 });

                    // We pick the intersections that are not presents in the configuration
                    var newIntesections = intersections.Where(intersection => !IsPointInContainer(container, firstContainer, intersection));

                    // Add register all of them to the dictionary
                    foreach (var newIntesection in newIntesections)
                    {
                        // Pull the set of passing objects
                        var passingObjects = dictionary.GetOrAdd(newIntesection, () => new HashSet<GeometricalObject>());

                        // Add our lines/circles to it
                        passingObjects.Add(lineOrCircle1);
                        passingObjects.Add(lineOrCircle2);
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
        /// <returns>true, if the point is one of the contextual container's points; false otherwise.</returns>
        private bool IsPointInContainer(IContextualContainer contextualContainer, IObjectsContainer objectsContainer, Point point)
        {
            // For a given point, find the configuration object version in the container
            var configurationObject = objectsContainer.Get(point);

            // It's present if it's not null and the contextual container actually contains it
            return configurationObject != null && contextualContainer.Contains(configurationObject);
        }

        #endregion
    }
}