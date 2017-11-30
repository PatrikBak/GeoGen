using System.Collections;
using System.Collections.Generic;
using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer.Theorems
{
    internal sealed class SimpleTheoremContainer : ITheoremsContainer
    {
        private readonly HashSet<Theorem> _theorems;

        public SimpleTheoremContainer()
        {
            _theorems = new HashSet<Theorem>();
        }

        public void Add(Theorem theorem)
        {
            _theorems.Add(theorem);
        }

        public bool Contains(Theorem theorem)
        {
            return _theorems.Contains(theorem);
        }

        public IEnumerator<Theorem> GetEnumerator()
        {
            return _theorems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}