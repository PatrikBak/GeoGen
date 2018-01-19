using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    internal  class TheoremsVerifier : ITheoremsVerifier
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

        public IEnumerable<Theorem> FindTheorems(IReadOnlyList<ConfigurationObject> oldObjects, IReadOnlyList<ConstructedConfigurationObject> newObjects)
        {
            var oldObjectsMap = new ConfigurationObjectsMap(oldObjects);

            var newObjectsMap = new ConfigurationObjectsMap(newObjects);

            var container = _containersManager.ExecuteAndResolvePossibleIncosistencies(() =>
            {
                var r = new ContextualContainer(_containersManager, new AnalyticalHelper());

                foreach (var configurationObject in oldObjects.Concat(newObjects))
                {
                    r.Add(configurationObject);
                }

                return r;
            });


            var input = new VerifierInput(container, oldObjectsMap, newObjectsMap);

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