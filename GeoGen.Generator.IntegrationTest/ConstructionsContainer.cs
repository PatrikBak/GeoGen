using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Constructions;

namespace GeoGen.Generator.IntegrationTest
{
    internal class ConstructionsContainer
    {
        private readonly Dictionary<Type, PredefinedConstruction> _predefinedConstructions;

        private readonly List<ComposedConstruction> _composedConstructions;

        private int _lastId;

        public ConstructionsContainer()
        {
            _predefinedConstructions = new Dictionary<Type, PredefinedConstruction>();
            _composedConstructions = new List<ComposedConstruction>();
        }

        public Construction Get<T>() where T : PredefinedConstruction, new()
        {
            if (_predefinedConstructions.ContainsKey(typeof(T)))
                return _predefinedConstructions[typeof(T)];

            var instance = new T {Id = _lastId++};

            _predefinedConstructions.Add(typeof(T), instance);

            return instance;
        }

        public void Add(ComposedConstruction composedConstruction)
        {
            composedConstruction.Id = _lastId++;

            _composedConstructions.Add(composedConstruction);
        }

        public IEnumerable<Construction> Constructions => _predefinedConstructions.Values.Cast<Construction>().Concat(_composedConstructions);
    }
}