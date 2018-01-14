using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;
using GeoGen.Utilities;
using GeoGen.Utilities.Subsets;

namespace GeoGen.Analyzer
{
    public class Wtf
    {
        public static int Possibilities;
        public static int Inconsistencies;
        public static int MaximalNeededAttemps;
        public static int MaximalContainerIterations;

        public static void Reset()
        {
            Possibilities = 0;
            Inconsistencies = 0;
            MaximalNeededAttemps = 0;
        }
    }

    internal sealed class ConcurrencyVerifier : ITheoremVerifier
    {
        public TheoremType TheoremType { get; } = TheoremType.ConcurrentObjects;

        private readonly IAnalyticalHelper _helper;

        private readonly ISubsetsProvider _provider;

        public ConcurrencyVerifier(IAnalyticalHelper helper, ISubsetsProvider provider)
        {
            _helper = helper ?? throw new ArgumentNullException(nameof(helper));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
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
                        Wtf.Possibilities++;

                        var involdedObjects = subsetOfOldObjects.Concat(newObjectsList).ToList();

                        bool Verify(IObjectsContainer container)
                        {
                            var analyticalObjects = involdedObjects
                                    .Select(obj => verifierInput._container.GetAnalyticalObject(obj, container))
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