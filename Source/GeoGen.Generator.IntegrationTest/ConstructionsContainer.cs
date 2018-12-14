using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GeoGen.Core;

namespace GeoGen.Generator.IntegrationTest
{
    internal class ConstructionsContainer
    {
        private readonly Dictionary<PredefinedConstructionType, PredefinedConstruction> _predefinedConstructions;

        private readonly List<ComposedConstruction> _composedConstructions;

        private readonly Dictionary<int, string> _names;

        private int _lastId;

        public ConstructionsContainer()
        {
            _predefinedConstructions = new Dictionary<PredefinedConstructionType, PredefinedConstruction>();
            _composedConstructions = new List<ComposedConstruction>();
            _names = new Dictionary<int, string>();
        }

        public Construction Get(PredefinedConstructionType type)
        {
            if (_predefinedConstructions.ContainsKey(type))
                return _predefinedConstructions[type];

            var construction = PredefinedConstructionsFactory.Get(type);
            construction.Id = _lastId++;
            _predefinedConstructions.Add(type, construction);
            _names.Add(_lastId - 1, ExtractName(type));

            return construction;
        }

        public Construction Get(string name)
        {
            return _composedConstructions.First(c => GetName(c) == name);
        }

        private string ExtractName(PredefinedConstructionType type) => Regex.Match(type.ToString(), "(.*)From.*").Groups[1].Value;

        public void Add(ComposedConstruction composedConstruction)
        {
            composedConstruction.Id = _lastId++;

            _composedConstructions.Add(composedConstruction);

            _names.Add(_lastId - 1, composedConstruction.Name);
        }

        public string GetName(Construction construction) => _names[construction.Id ?? throw new Exception()];
    }
}