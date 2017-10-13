using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Analyzer.Objects;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Analyzer.Objects.GeometricalObjects.Container;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.SubsetsProviding;

namespace GeoGen.Analyzer.Theorems.TheoremVerifiers
{
    internal class ConcurrencyVerifier : TheoremVerifierBase
    {
        public override TheoremType TheoremType { get; } = TheoremType.ConcurrentLines;

        private readonly IContextualContainer _contextualContainer;

        private readonly IAnalyticalHelper _analyticalHelper;

        private readonly ISubsetsProvider<GeometricalObject> _subsetsProvider;

        public ConcurrencyVerifier
        (
            ITheoremObjectConstructor theoremObjectConstructor,
            IContextualContainer contextualContainer,
            IAnalyticalHelper analyticalHelper,
            ISubsetsProvider<GeometricalObject> subsetsProvider
        )
            : base(theoremObjectConstructor)
        {
            _contextualContainer = contextualContainer;
            _analyticalHelper = analyticalHelper;
            _subsetsProvider = subsetsProvider;
        }

        public override IEnumerable<VerifierOutput> GetOutput(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects)
        {
            var allObjectsMap = oldObjects.Merge(newObjects);

            var newLines = _contextualContainer.GetNewObjects<CircleObject>(oldObjects, newObjects);
            var newCircles = _contextualContainer.GetNewObjects<LineObject>(oldObjects, newObjects);
            var newLinesAndCircles = newLines.Concat(newCircles.Cast<GeometricalObject>()).ToList();

            var oldLines = _contextualContainer.GetObjects<LineObject>(oldObjects);
            var oldCircles = _contextualContainer.GetObjects<CircleObject>(oldObjects);
            var oldLinesAndCircles = oldLines.Concat(oldCircles.Cast<GeometricalObject>()).ToList();

            var maxSize = Math.Max(newLinesAndCircles.Count, 3);

            for (var size = 1; size <= maxSize; size++)
            {
                foreach (var subsetOfNewObjects in _subsetsProvider.GetSubsets(newLinesAndCircles, size))
                {
                    var newObjectsList = subsetOfNewObjects.ToList();

                    var neededCount = 3 - size;

                    foreach (var subsetOfOldObjects in _subsetsProvider.GetSubsets(oldLinesAndCircles, neededCount))
                    {
                        var allObjects = subsetOfOldObjects.Concat(newObjectsList).ToList();

                        bool Verify(IObjectsContainer container)
                        {
                            var analyticalObjects = allObjects
                                    .Select(obj => _contextualContainer.GetAnalyticalObject(obj, container))
                                    .ToSet();

                            var allObjectsAnalytical = allObjectsMap
                                    .AllObjects()
                                    .Select(container.Get)
                                    .ToSet();

                            var newIntersections = _analyticalHelper
                                    .Intersect(analyticalObjects)
                                    .Where(allObjectsAnalytical.Contains);

                            return newIntersections.Empty();
                        }

                        yield return new VerifierOutput
                        {
                            Theorem = () => CreateTheorem(allObjectsMap, allObjects),
                            VerifierFunction = Verify
                        };
                    }
                }
            }
        }
    }
}