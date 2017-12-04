using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal sealed class TheoremsContainer : ITheoremsContainer
    {
        private class TheoremObjectComparer : IEqualityComparer<TheoremObject>
        {
            public static readonly TheoremObjectComparer Instance = new TheoremObjectComparer();

            public bool Equals(TheoremObject x, TheoremObject y)
            {
                // We assume the objects are not null. This should be handled above
                Debug.Assert(x != null, "x != null");
                Debug.Assert(y != null, "y != null");

                // If we have distinct then the objects are distinct
                if (x.Type != y.Type)
                    return false;

                // Local function to convert object to set of involved objects' ids
                HashSet<int> ToIdsSet(TheoremObject obj)
                {
                    return obj.InternalObjects
                            .Select(o => o.Id ?? throw new AnalyzerException("Id must be set"))
                            .ToSet();
                }

                // We convert both objects to these sets and compare them as sets
                return ToIdsSet(x).SetEquals(ToIdsSet(y));
            }

            public int GetHashCode(TheoremObject obj)
            {
                // Get the hash code of the signature
                var typeHash = obj.Type.GetHashCode();

                // Get the order-independent hash code of involved objects
                var objectsHash = obj.InternalObjects
                        .GetOrderIndependentHashCode(o => o.Id ?? throw new AnalyzerException("Id must be set"));

                // Get order-dependent hash code of these two values
                return new[] {typeHash, objectsHash}.GetOrderDependentHashCode(i => i);
            }
        }

        private class TheoremEqualityComparer : IEqualityComparer<Theorem>
        {
            public static readonly TheoremEqualityComparer Instance = new TheoremEqualityComparer();

            public bool Equals(Theorem x, Theorem y)
            {
                Debug.Assert(x != null, "x != null");
                Debug.Assert(y != null, "y != null");
                Debug.Assert(x.Type == y.Type, "x.Type == y.Type");

                return x.InvolvedObjects.ToSet(TheoremObjectComparer.Instance)
                        .SetEquals(y.InvolvedObjects.ToSet(TheoremObjectComparer.Instance));
            }

            public int GetHashCode(Theorem obj)
            {
                return obj.InvolvedObjects.GetOrderIndependentHashCode(TheoremObjectComparer.Instance.GetHashCode);
            }
        }

        private readonly IDictionary<TheoremType, CacheBasedContainer<Theorem>> _theoremsDictionary;

        public TheoremsContainer()
        {
            _theoremsDictionary = new Dictionary<TheoremType, CacheBasedContainer<Theorem>>();

            foreach (var value in Enum.GetValues(typeof(TheoremType)))
            {
                _theoremsDictionary.Add((TheoremType) value, new CacheBasedContainer<Theorem>(TheoremEqualityComparer.Instance));
            }
        }

        public void Add(Theorem theorem)
        {
            _theoremsDictionary[theorem.Type].Add(theorem);
        }

        public bool Contains(Theorem theorem)
        {
            return _theoremsDictionary[theorem.Type].Contains(theorem);
        }

        public IEnumerator<Theorem> GetEnumerator()
        {
            return _theoremsDictionary.Values.SelectMany(s => s).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}