using System;
using System.Collections.Generic;
using GeoGen.Core.Constructions;

namespace GeoGen.Analyzer.Constructing
{
    internal sealed class ConstructorsResolver : IConstructorsResolver
    {
        private readonly Dictionary<Type, IPredefinedConstructor> _constructors;

        public ConstructorsResolver(IPredefinedConstructor[] constructors)
        {
            if (constructors == null)
                throw new ArgumentNullException(nameof(constructors));

            _constructors = new Dictionary<Type, IPredefinedConstructor>();

            foreach (var constructor in constructors)
            {
                if (constructor == null)
                    throw new ArgumentException("Constructors can't contain null objects");

                var type = constructor.PredefinedConstructionType;

                if (_constructors.ContainsKey(type))
                    throw new ArgumentException("Duplicate constructors");

                _constructors.Add(type, constructor);
            }
        }

        public IObjectsConstructor Resolve(Construction construction)
        {
            if (construction == null)
                throw new ArgumentNullException(nameof(construction));

            if (construction is PredefinedConstruction predefinedConstruction)
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

            throw new NotImplementedException();
        }
    }
}