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
        public TheoremType TheoremType { get; } = TheoremType.ConcurrentLines;

        private readonly IContextualContainer _contextualContainer;

        private readonly IAnalyticalHelper _analyticalHelper;

        private readonly ISubsetsProvider _subsetsProvider;

        public ConcurrencyVerifier
        (
            IContextualContainer contextualContainer,
            IAnalyticalHelper analyticalHelper,
            ISubsetsProvider subsetsProvider
        )
        {
            _contextualContainer = contextualContainer ?? throw new ArgumentNullException(nameof(contextualContainer));
            _analyticalHelper = analyticalHelper ?? throw new ArgumentNullException(nameof(analyticalHelper));
            _subsetsProvider = subsetsProvider ?? throw new ArgumentNullException(nameof(subsetsProvider));
        }

        public IEnumerable<VerifierOutput> GetOutput(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects)
        {
            if (oldObjects == null)
                throw new ArgumentNullException(nameof(oldObjects));

            if (newObjects == null)
                throw new ArgumentNullException(nameof(newObjects));

            var allObjectsMap = oldObjects.Merge(newObjects);

            var newLines = _contextualContainer.GetNewObjects<CircleObject>(oldObjects, newObjects);
            var newCircles = _contextualContainer.GetNewObjects<LineObject>(oldObjects, newObjects);
            var newLinesAndCircles = newLines.Concat(newCircles.Cast<GeometricalObject>()).ToList();

            var oldLines = _contextualContainer.GetObjects<LineObject>(oldObjects);
            var oldCircles = _contextualContainer.GetObjects<CircleObject>(oldObjects);
            var oldLinesAndCircles = oldLines.Concat(oldCircles.Cast<GeometricalObject>()).ToList();

            var maxSize = Math.Min(newLinesAndCircles.Count, 3);

            for (var size = 1; size <= maxSize; size++)
            {
                foreach (var subsetOfNewObjects in _subsetsProvider.GetSubsets(newLinesAndCircles, size))
                {
                    var newObjectsList = subsetOfNewObjects.ToList();

                    var neededCount = 3 - size;

                    foreach (var subsetOfOldObjects in _subsetsProvider.GetSubsets(oldLinesAndCircles, neededCount))
                    {
                        var involdedObjects = subsetOfOldObjects.Concat(newObjectsList).ToList();

                        bool Verify(IObjectsContainer container)
                        {
                            var analyticalObjects = involdedObjects
                                    .Select(obj => _contextualContainer.GetAnalyticalObject(obj, container))
                                    .ToSet();

                            var allPointsAnalytical = allObjectsMap[ConfigurationObjectType.Point]
                                    .Select(container.Get)
                                    .Cast<Point>()
                                    .ToSet();

                            var newIntersections = _analyticalHelper
                                    .Intersect(analyticalObjects)
                                    .Where(p => !allPointsAnalytical.Contains(p));

                            return newIntersections.Any();
                        }

                        yield return new VerifierOutput
                        {
                            AllObjects = allObjectsMap,
                            InvoldedObjects = involdedObjects,
                            VerifierFunction = Verify
                        };
                    }
                }
            }
        }
    }
}