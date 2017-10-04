using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.SubsetsProviding;
using GeoGen.Core.Utilities.VariationsProviding;
using static GeoGen.Core.Configurations.ConfigurationObjectType;

namespace GeoGen.Analyzer.Theorems
{
    internal class TheoremsFinder : ITheoremsFinder
    {
        private readonly ITheoremVerifier[] _verifiers;

        private readonly IGeometryHolder _holder;

        private readonly ISubsetsProvider<ConfigurationObject> _subsetsProvider;

        private readonly IVariationsProvider<ConfigurationObject> _variationsProvider;

        public TheoremsFinder(ITheoremVerifier[] verifiers, IGeometryHolder holder, ISubsetsProvider<ConfigurationObject> subsetsProvider, IVariationsProvider<ConfigurationObject> variationsProvider)
        {
            _verifiers = verifiers;
            _holder = holder;
            _subsetsProvider = subsetsProvider;
            _variationsProvider = variationsProvider;
        }

        public IEnumerable<Theorem> Find(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects)
        {
            foreach (var verifier in _verifiers)
            {
                var counter = 0;

                foreach (var signature in verifier.Signatures)
                {
                    foreach (var input in GeneratePossibleInputs(signature, oldObjects, newObjects))
                    {
                        var holdsTrue = _holder.All(container => verifier.Verify(input, counter, container));

                        if (holdsTrue)
                            yield return verifier.ConstructTheorem(input, counter);
                    }

                    counter++;
                }
            }
        }

        private IEnumerable<ConfigurationObjectsMap> GeneratePossibleInputs
        (
            Dictionary<ConfigurationObjectType, int> signature,
            ConfigurationObjectsMap oldObjects,
            ConfigurationObjectsMap newObjects
        )
        {
            var canPerformVerification = true;

            foreach (var pair in signature)
            {
                var type = pair.Key;
                var neededCount = pair.Value;

                if (oldObjects.CountOfType(type) + newObjects.CountOfType(type) < neededCount)
                {
                    canPerformVerification = false;
                    break;
                }
            }

            if (!canPerformVerification)
                yield break;

            int NeededObjects(ConfigurationObjectType type) => signature.ContainsKey(type) ? signature[type] : 0;

            foreach (var newPoints in NewObjectsForType(Point, signature, oldObjects, newObjects))
            {
                foreach (var newLines in NewObjectsForType(Line, signature, oldObjects, newObjects))
                {
                    foreach (var newCircles in NewObjectsForType(Circle, signature, oldObjects, newObjects))
                    {
                        if (newPoints.Empty() && newLines.Empty() && newCircles.Empty())
                            continue;

                        var neededPoints = NeededObjects(Point) - newPoints.Count;
                        var neededLines = NeededObjects(Line) - newLines.Count;
                        var neededCircles = NeededObjects(Circle) - newCircles.Count;

                        foreach (var oldPoints in OldObjectsForType(Point, neededPoints, oldObjects))
                        {
                            foreach (var oldLines in OldObjectsForType(Line, neededLines, oldObjects))
                            {
                                foreach (var oldCircles in OldObjectsForType(Circle, neededCircles, oldObjects))
                                {
                                    var allObjects = newPoints
                                            .Concat(newLines)
                                            .Concat(newCircles)
                                            .Concat(oldPoints)
                                            .Concat(oldLines)
                                            .Concat(oldCircles)
                                            .ToList();

                                    var permutations = _variationsProvider.GetVariations(allObjects, allObjects.Count);

                                    foreach (var permutation in permutations)
                                    {
                                        yield return new ConfigurationObjectsMap(permutation);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<List<ConfigurationObject>> NewObjectsForType
        (
            ConfigurationObjectType type,
            Dictionary<ConfigurationObjectType, int> signature,
            ConfigurationObjectsMap oldObjects,
            ConfigurationObjectsMap newObjects
        )
        {
            if (!signature.ContainsKey(type) || !newObjects.ContainsKey(type))
            {
                yield return new List<ConfigurationObject>();
                yield break;
            }

            var newObjectsOfType = newObjects[type];

            var neededObjects = signature.ContainsKey(type) ? signature[type] : 0;

            var lowerBound = Math.Max(0, neededObjects - oldObjects.CountOfType(type));
            var upperBound = Math.Min(signature[type], newObjectsOfType.Count);

            for (var count = lowerBound; count <= upperBound; count++)
            {
                foreach (var objects in _subsetsProvider.GetSubsets(newObjectsOfType, count))
                {
                    yield return objects.ToList();
                }
            }
        }

        IEnumerable<List<ConfigurationObject>> OldObjectsForType
        (
            ConfigurationObjectType type,
            int count,
            ConfigurationObjectsMap oldObjects
        )
        {
            if (!oldObjects.ContainsKey(type))
            {
                yield return new List<ConfigurationObject>();
                yield break;
            }

            var oldObjectOfType = oldObjects[type];

            foreach (var objects in _subsetsProvider.GetSubsets(oldObjectOfType, count))
            {
                yield return objects.ToList();
            }
        }
    }
}