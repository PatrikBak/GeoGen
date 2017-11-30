using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Objects;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal sealed class TheoremsVerifier : ITheoremsVerifier
    {
        private readonly IObjectsContainersManager _containers;

        private readonly ITheoremVerifier[] _verifiers;

        public TheoremsVerifier(IObjectsContainersManager containers, ITheoremVerifier[] verifiers)
        {
            _containers = containers;
            _verifiers = verifiers;
        }

        public IEnumerable<Theorem> FindTheorems(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects)
        {
            foreach (var theoremVerifier in _verifiers)
            {
                var output = theoremVerifier.GetOutput(oldObjects, newObjects);

                foreach (var verifierOutput in output)
                {
                    if (_containers.All(container => verifierOutput.VerifierFunction(container)))
                    {
                        yield return CreateTheorem(verifierOutput);
                    }
                }
            }
        }

        private static Theorem CreateTheorem(VerifierOutput verifierOutput)
        {
            var objects = verifierOutput.InvoldedObjects
                    .Select(obj => Construct(verifierOutput.AllObjects, obj))
                    .ToList();

            return new Theorem(verifierOutput.TheoremType, objects);
        }

        private static TheoremObject Construct(ConfigurationObjectsMap objects, GeometricalObject geometricalObject)
        {
            if (geometricalObject is PointObject)
                return new TheoremObject(geometricalObject.ConfigurationObject);

            var line = geometricalObject as LineObject;
            var circle = geometricalObject as CircleObject;

            var type = line != null
                ? ConfigurationObjectType.Line
                : ConfigurationObjectType.Circle;

            var configurationObject = geometricalObject.ConfigurationObject;

            if (configurationObject != null && objects[type].Contains(configurationObject))
            {
                return new TheoremObject(configurationObject);
            }

            var points = line != null
                ? line.Points
                : circle?.Points ?? throw new AnalyzerException("Unhandled case");

            var involedObjects = points
                    .Select(point => point.ConfigurationObject)
                    .Where(point => objects[ConfigurationObjectType.Point].Contains(point))
                    .ToList();

            var objectType = line != null
                ? TheoremObjectSignature.LineGivenByPoints
                : TheoremObjectSignature.CircleGivenByPoints;

            return new TheoremObject(involedObjects, objectType);
        }
    }
}