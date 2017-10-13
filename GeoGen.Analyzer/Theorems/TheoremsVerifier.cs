using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal sealed class TheoremsVerifier : ITheoremsVerifier
    {
        private readonly IObjectsContainersHolder _containers;

        private readonly ITheoremVerifier[] _verifiers;

        public TheoremsVerifier(IObjectsContainersHolder containers, ITheoremVerifier[] verifiers)
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
                        yield return verifierOutput.Theorem();
                    }
                }
            }
        }
    }
}