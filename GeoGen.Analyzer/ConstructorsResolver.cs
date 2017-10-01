using System;
using System.Collections.Generic;
using GeoGen.Core.Constructions;

namespace GeoGen.Analyzer
{
    internal class ConstructorsResolver : IConstructorsResolver
    {
        private readonly Dictionary<Type, IPredefinedConstructor> _constructors;

        public ConstructorsResolver(IPredefinedConstructor[] constructors)
        {
            if (constructors == null)
                throw new ArgumentNullException(nameof(constructors));

            _constructors = new Dictionary<Type, IPredefinedConstructor>();

            foreach (var constructor in constructors)
            {
                var type = constructor.PredefinedConstructionType;

                _constructors.Add(type, constructor);
            }
        }

        public IPredefinedConstructor Resolve(PredefinedConstruction predefinedConstruction)
        {
            try
            {
                return _constructors[predefinedConstruction.GetType()];
            }
            catch (KeyNotFoundException)
            {
                throw new AnalyzerException("Unresolvable predefined constructor.");
            }
        }
    }
}