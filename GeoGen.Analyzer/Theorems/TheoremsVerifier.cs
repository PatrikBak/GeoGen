using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Objects;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Analyzer.Objects.GeometricalObjects.Container;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal sealed class TheoremsVerifier : ITheoremsVerifier
    {
        private readonly ITheoremVerifier[] _verifiers;

        private readonly IObjectsContainersManager _containersManager;

        private readonly ITheoremsContainer _theoremsContainer;

        private readonly IContextualContainer _contextualContainer;

        public TheoremsVerifier
        (
            ITheoremVerifier[] verifiers,
            IObjectsContainersManager containersManager,
            ITheoremsContainer theoremsContainer,
            IContextualContainer contextualContainer
        )
        {
            _verifiers = verifiers;
            _containersManager = containersManager;
            _theoremsContainer = theoremsContainer;
            _contextualContainer = contextualContainer;
        }

        public IEnumerable<Theorem> FindTheorems(List<ConfigurationObject> oldObjects, List<ConstructedConfigurationObject> newObjects)
        {
            var oldObjectsMap = new ConfigurationObjectsMap(oldObjects);

            var newObjectsMap = new ConfigurationObjectsMap(newObjects);

            var input = new VerifierInput(_contextualContainer, oldObjectsMap, newObjectsMap);

            foreach (var theoremVerifier in _verifiers)
            {
                var theoremType = theoremVerifier.TheoremType;

                var output = theoremVerifier.GetOutput(input);

                foreach (var verifierOutput in output)
                {
                    var verifierFunction = verifierOutput.VerifierFunction;

                    var isTrue = verifierFunction == null || _containersManager.All(verifierFunction);

                    if (!isTrue)
                        continue;

                    var theoremObjects = verifierOutput.InvoldedObjects
                            .Select(obj => Construct(input.AllObjects, obj))
                            .ToSet();

                    var theorem = new Theorem(theoremType, theoremObjects);

                    if (!_theoremsContainer.Contains(theorem))
                    {
                        yield return theorem;
                    }
                }
            }
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

            var points = line?.Points ?? circle?.Points ?? throw new AnalyzerException("Unhandled case");

            var pointIds = points
                    .Select(p => p.ConfigurationObject.Id ?? throw new AnalyzerException("Id must be set"))
                    .ToSet();

            var involedObjects = objects[ConfigurationObjectType.Point]
                    .Where(point => pointIds.Contains(point.Id ?? throw new AnalyzerException("Id must be set")))
                    .ToList();

            var objectType = line != null
                ? TheoremObjectSignature.LineGivenByPoints
                : TheoremObjectSignature.CircleGivenByPoints;

            return new TheoremObject(involedObjects, objectType);
        }
    }
}