using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal abstract class TheoremVerifierBase : ITheoremVerifier
    {
        public abstract TheoremType TheoremType { get; }

        public abstract IEnumerable<VerifierOutput> GetOutput(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects);

        private readonly ITheoremObjectConstructor _theoremObjectConstructor;

        protected TheoremVerifierBase(ITheoremObjectConstructor theoremObjectConstructor)
        {
            _theoremObjectConstructor = theoremObjectConstructor;
        }

        protected Theorem CreateTheorem(ConfigurationObjectsMap allObjects, List<GeometricalObject> involdedObjects)
        {
            var objects = involdedObjects
                    .Select(obj => _theoremObjectConstructor.Construct(allObjects, obj))
                    .ToList();

            return new Theorem(TheoremType, objects);
        }
    }
}