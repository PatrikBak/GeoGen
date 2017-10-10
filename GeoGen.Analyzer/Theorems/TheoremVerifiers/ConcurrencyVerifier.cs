using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Analyzer.AnalyticalGeometry;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Objects;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.SubsetsProviding;

namespace GeoGen.Analyzer.Theorems.TheoremVerifiers
{
    internal class ConcurrencyVerifier : ITheoremVerifier
    {
        private ICoolInterface _contextualContainer;

        private IIntersector _intersector;

        private ISubsetsProvider<GeometricalObject> _subsetsProvider;

        public IEnumerable<VerifierOutput> GetOutput(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects)
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

                            var newIntersections = _intersector
                                    .Intersect(analyticalObjects)
                                    .Where(point => _contextualContainer.IsContained(point, allObjectsMap));

                            return newIntersections.Empty();
                        }

                        Theorem Theorem()
                        {
                            var configurationObjects = allObjects
                                    .Select(obj => _contextualContainer.GetContainedObjects(allObjectsMap, obj));

                            return null;

                        }

                        yield return new VerifierOutput
                        {
                            Theorem = Theorem,
                            VerifierFunction = Verify
                        };
                    }
                }
            }
        }

        private Theorem CreateTheorem(List<GeometricalObject> allObjects)
        {
            return null;
        }
    }
}