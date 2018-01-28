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
                var involdedObjects = objects.ToList();

                bool Verify(IObjectsContainer objectsContainer)
                {
                    var analyticalObjects = involdedObjects
                            .Select(obj => container.GetAnalyticalObject(obj, objectsContainer))
                            .ToSet();

                    var allPointsAnalytical = container
                            .GetGeometricalObjects<PointObject>()
                            .Select(o => objectsContainer.Get(o.ConfigurationObject))
                            .Cast<Point>()
                            .ToSet();

                    var newIntersections = _helper
                            .Intersect(analyticalObjects)
                            .Where(p => !allPointsAnalytical.Contains(p));

                    return newIntersections.Any();
                }

                yield return new VerifierOutput
                {
                    Type = TheoremType.ConcurrentObjects,
                    InvoldedObjects = involdedObjects,
                    AlwaysTrue = false,
                    VerifierFunction = Verify
                };
            }
        }
    }
}