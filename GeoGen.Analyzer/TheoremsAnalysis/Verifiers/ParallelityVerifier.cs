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
    internal class ParallelityVerifier : ITheoremVerifier
    {
        #region Private fields

        /// <summary>
        /// The helper for intersecting analytical objects.
        /// </summary>
        private readonly IAnalyticalHelper _helper;

        /// <summary>
        /// The manager of all objects containers.
        /// </summary>
        private readonly IObjectsContainersManager _manager;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="helper">The analytical helper.</param>
        /// <param name="manager">The manager for containers.</param>
        public ParallelityVerifier(IAnalyticalHelper helper, IObjectsContainersManager manager)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
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
            // Find all new lines. At least one of them must be included in a new theorem
            var newLines = container.GetGeometricalObjects<GeometricalObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludePoints = false,
                IncludeLines = true,
                IncludeCirces = false
            }).ToList();

            // Find all lines. These will be combined with the new ones
            var allLines = container.GetGeometricalObjects<GeometricalObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.All,
                IncludePoints = false,
                IncludeLines = true,
                IncludeCirces = false
            }).ToList();

            // Prepare set of all pairs of parallel lines in the first container
            var lines = new HashSet<Tuple<GeometricalObject, GeometricalObject>>();

            // Pull first container
            var firstContainer = _manager.First();

            // Iterate over lines to get all pairs
            foreach (var line1 in newLines)
            {
                foreach (var line2 in allLines)
                {
                    // If they are equal, we skip them
                    if (Equals(line1, line2))
                        continue;

                    // Pull their analytical version
                    var analytical1 = container.GetAnalyticalObject(line1, firstContainer);
                    var analytical2 = container.GetAnalyticalObject(line2, firstContainer);

                    // Intersect them
                    var intersections = _helper.Intersect(new List<AnalyticalObject> { analytical1, analytical2 });

                    // If there is an intersection, skip them
                    if (intersections.Any())
                        continue;

                    // We find the line with the smaller id
                    var withSmallerId = line1.Id < line2.Id ? line1 : line2;
                    var withLargerId = withSmallerId == line1 ? line2 : line1;

                    // And add this pair of lines to the lines set
                    lines.Add(new Tuple<GeometricalObject, GeometricalObject>(withSmallerId, withLargerId));
                }
            }

            // After finding the lines paralellel in the first container, we want to have
            // a look to the others. We go through the lines set
            foreach (var pair in lines)
            {
                // Construct involved objects
                var involvedObjects = pair.Item1.AsEnumerable().Concat(pair.Item2.AsEnumerable()).ToList();

                // Construct the verifier function
                bool Verify(IObjectsContainer objectsContainer)
                {
                    // If the container is the first one, we're sure the lines are fine
                    if (ReferenceEquals(firstContainer, objectsContainer))
                        return true;

                    // Otherwise we find analytical versions of our lines
                    var analyticalObjects = involvedObjects.Select(obj => container.GetAnalyticalObject(obj, objectsContainer));

                    // Intersect them
                    var intersections = _helper.Intersect(analyticalObjects);

                    // The output is correct if there is no intersection
                    return intersections.Empty();
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

        #endregion
    }
}