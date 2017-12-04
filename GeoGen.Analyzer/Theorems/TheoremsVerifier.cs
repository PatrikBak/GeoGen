using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

        private readonly IContextualContainer _contextualContainer;

        private readonly ITheoremConstructor _constructor;

        public TheoremsVerifier
        (
            ITheoremVerifier[] verifiers,
            IObjectsContainersManager containersManager,
            IContextualContainer contextualContainer,
            ITheoremConstructor constructor
        )
        {
            _verifiers = verifiers ?? throw new ArgumentNullException(nameof(verifiers));
            _containersManager = containersManager ?? throw new ArgumentNullException(nameof(containersManager));
            _contextualContainer = contextualContainer ?? throw new ArgumentNullException(nameof(contextualContainer));
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));

            if (_verifiers.Contains(null))
                throw new ArgumentException("Null theorem verifier passed to TheoremsVerifier constructor.");
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

                    var theorem = _constructor.Construct(verifierOutput.InvoldedObjects, input.AllObjects, theoremType);

                    yield return theorem;
                }
            }
        }
    }
}