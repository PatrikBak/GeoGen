using GeoGen.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GeoGen.ConsoleTest
{
    public class ConstructionsContainer
    {
        private readonly Dictionary<PredefinedConstructionType, PredefinedConstruction> _predefinedConstructions;

        private readonly List<ComposedConstruction> _composedConstructions;

        private readonly List<(Construction, string)> _names;

        public ConstructionsContainer()
        {
            _predefinedConstructions = new Dictionary<PredefinedConstructionType, PredefinedConstruction>();
            _composedConstructions = new List<ComposedConstruction>();
            _names = new List<(Construction, string)>();
        }

        public Construction Get(PredefinedConstructionType type)
        {
            if (_predefinedConstructions.ContainsKey(type))
                return _predefinedConstructions[type];

            var construction = PredefinedConstructionsFactory.Get(type);
            _predefinedConstructions.Add(type, construction);
            _names.Add((construction, ExtractName(type)));

            return construction;
        }

        public Construction Get(string name)
        {
            return _composedConstructions.First(c => GetName(c) == name);
        }

        private string ExtractName(PredefinedConstructionType type) => Regex.Match(type.ToString(), "(.*)From.*").Groups[1].Value;

        public void Add(ComposedConstruction composedConstruction)
        {
            _composedConstructions.Add(composedConstruction);
            _names.Add((composedConstruction, composedConstruction.Name));
        }

        public string GetName(Construction construction) => _names.Where(pair => pair.Item1 == construction).First().Item2;
    }
}