using GeoGen.AnalyticalGeometry;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="ITheoremVerifier"/> for <see cref="TheoremType.EqualLineSegments"/>.
    /// </summary>
    internal class EqualLineSegmentsVerifier : TheoremVerifierBase
    {
        #region Private fields

        /// <summary>
        /// The generator of subsets of given length.
        /// </summary>
        private readonly ISubsetsProvider _provider;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="provider">The subsets generator.</param>
        public EqualLineSegmentsVerifier(ISubsetsProvider provider)
        {
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
        public override IEnumerable<VerifierOutput> GetOutput(IContextualContainer container)
        {
            // Find new points. 
            var newPoints = container.GetGeometricalObjects<PointObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludePoints = true,
                IncludeLines = false,
                IncludeCirces = false
            }).ToList();

            // Find all points. 
            var allPoints = container.GetGeometricalObjects<PointObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.All,
                IncludePoints = true,
                IncludeLines = false,
                IncludeCirces = false
            }).ToList();

            // Prepare map that maps distances between point tuples to the sets of 
            // line segments with this distance with regards to the first container,
            // represented as a set of two points
            var distancesMap = new Dictionary<RoundedDouble, HashSet<HashSet<PointObject>>>();

            // Pull first container
            var firstContainer = container.Manager.First();

            // Iterate over poins to get all pairs (with at least one being a new one)
            foreach (var point1 in newPoints)
            {
                foreach (var point2 in allPoints)
                {
                    // If they are equal, we skip them
                    if (Equals(point1, point2))
                        continue;

                    // Pull their analytical version
                    var analytical1 = (Point)container.GetAnalyticalObject(point1, firstContainer);
                    var analytical2 = (Point)container.GetAnalyticalObject(point2, firstContainer);

                    // Calculate their distance and round it.
                    var distance = (RoundedDouble)analytical1.DistanceTo(analytical2);

                    // Prepare the pair of them
                    var pair = new HashSet<PointObject> { point1, point2 };

                    // Find the right set where this pair should be added acorrding to the distance
                    var set = distancesMap.GetOrAdd(distance, () => new HashSet<HashSet<PointObject>>(SetComparer<PointObject>.Instance));

                    // Add the pair to the set
                    set.Add(pair);
                }
            }

            // We pull the values (i.e. the sets of line segments)
            // where there are at least two with the same distance
            var interestingSets = distancesMap.Values.Where(set => set.Count >= 2);

            // And for each of them
            foreach (var lineSegment in interestingSets)
            {
                // We have a given set of line segments. We want to find
                // all pair of their elements (so we can verify them against
                // all containers, not just the first one)
                foreach (var lineSegmentsPair in _provider.GetSubsets(lineSegment.ToList(), 2))
                {
                    // So far let's have involved objects as four points...
                    var involvedObjects = lineSegmentsPair.SelectMany(set => set).ToList();

                    // We may construct the verifier function now
                    // Construct the verifier function
                    bool Verify(IObjectsContainer objectsContainer)
                    {
                        // If the container is the first one, we're sure the objects are fine
                        if (ReferenceEquals(firstContainer, objectsContainer))
                            return true;

                        // Otherwise we find analytical versions of our objects and cast them to points
                        var analyticalObjects = involvedObjects
                            .Select(obj => container.GetAnalyticalObject(obj, objectsContainer))
                            .Cast<Point>()
                            .ToList();

                        // We're fine if the rounded distance between the first two is the same as 
                        // the rounded distance between the other two
                        var firstDistance = (RoundedDouble)analyticalObjects[0].DistanceTo(analyticalObjects[1]);
                        var secondDistance = (RoundedDouble)analyticalObjects[2].DistanceTo(analyticalObjects[3]);

                        // Return if they are the same
                        return firstDistance == secondDistance;
                    }

                    // Construct the output
                    yield return new VerifierOutput
                    {
                        Type = Type,
                        InvoldedObjects = involvedObjects,
                        AlwaysTrue = false,
                        VerifierFunction = Verify
                    };
                }
            }
        }

        #endregion
    }
}