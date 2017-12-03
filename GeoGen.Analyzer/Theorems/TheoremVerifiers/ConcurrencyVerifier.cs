using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.Analyzer.Objects;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Analyzer.Objects.GeometricalObjects.Container;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.SubsetsProviding;

namespace GeoGen.Analyzer.Theorems.TheoremVerifiers
{
    internal sealed class ConcurrencyVerifier : ITheoremVerifier
    {
        public TheoremType TheoremType { get; } = TheoremType.ConcurrentObjects;

        private readonly IAnalyticalHelper _helper;

        private readonly ISubsetsProvider _provider;

        private readonly IContextualContainer _container;

        public ConcurrencyVerifier(IAnalyticalHelper helper, ISubsetsProvider provider, IContextualContainer container)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public IEnumerable<VerifierOutput> GetOutput(VerifierInput verifierInput)
        {
            if (verifierInput == null)
                throw new ArgumentNullException(nameof(verifierInput));

            var allObjectsMap = verifierInput.AllObjects;

            var newLines = verifierInput.NewLines;
            var newCircles = verifierInput.NewCircles;
            var newLinesAndCircles = newLines.Concat(newCircles.Cast<GeometricalObject>()).ToList();

            var oldLines = verifierInput.OldLines;
            var oldCircles = verifierInput.OldCircles;
            var oldLinesAndCircles = oldLines.Concat(oldCircles.Cast<GeometricalObject>()).ToList();

            var maxSize = Math.Min(newLinesAndCircles.Count, 3);

            for (var size = 1; size <= maxSize; size++)
            {
                foreach (var subsetOfNewObjects in _provider.GetSubsets(newLinesAndCircles, size))
                {
                    var newObjectsList = subsetOfNewObjects.ToList();

                    var neededCount = 3 - size;

                    foreach (var subsetOfOldObjects in _provider.GetSubsets(oldLinesAndCircles, neededCount))
                    {
                        var involdedObjects = subsetOfOldObjects.Concat(newObjectsList).ToList();

                        bool Verify(IObjectsContainer container)
                        {
                            var analyticalObjects = involdedObjects
                                    .Select(obj => _container.GetAnalyticalObject(obj, container))
                                    .ToSet();

                            var allPointsAnalytical = allObjectsMap[ConfigurationObjectType.Point]
                                    .Select(container.Get)
                                    .Cast<Point>()
                                    .ToSet();

                            var newIntersections = _helper
                                    .Intersect(analyticalObjects)
                                    .Where(p => !allPointsAnalytical.Contains(p));

                            return newIntersections.Any();
                        }

                        yield return new VerifierOutput
                        {
                            InvoldedObjects = involdedObjects,
                            VerifierFunction = Verify
                        };
                    }
                }
            }
        }
    }
}