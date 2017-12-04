using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core.Utilities
{
    public class CacheBasedContainer<T> : IEnumerable<T>
    {
        private class WrappedValue
        {
            public T Value { get; }

            private readonly IEqualityComparer<T> _comparer;

            private readonly int _hashCode;

            public WrappedValue(T value, IEqualityComparer<T> comparer)
            {
                Value = value;
                _comparer = comparer;
                _hashCode = _comparer.GetHashCode(value);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;

                if (ReferenceEquals(this, obj))
                    return true;

                if (obj.GetType() != GetType())
                    return false;

                return _comparer.Equals(((WrappedValue) obj).Value, Value);
            }

            public override int GetHashCode()
            {
                return _comparer.GetHashCode(Value);
                //return _hashCode;
            }
        }

        private readonly IEqualityComparer<T> _equalityComparer;

        private readonly HashSet<WrappedValue> _values;

        public CacheBasedContainer(IEqualityComparer<T> equalityComparer)
        {
            _equalityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
            _values = new HashSet<WrappedValue>();
        }

        public void Add(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            _values.Add(new WrappedValue(value, _equalityComparer));
        }

        public bool Contains(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return _values.Contains(new WrappedValue(value, _equalityComparer));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _values.Select(v => v.Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}