using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoGen.Core.Utilities
{
    public class Map<T1, T2>
    {
        public class Indexer<T3, T4>
        {
            private readonly Dictionary<T3, T4> _dictionary;

            public Indexer(Dictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }

            public T4 this[T3 index]
            {
                get => _dictionary[index];
                set => _dictionary[index] = value;
            }
        }

        private readonly Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();

        private readonly Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

        public Indexer<T1, T2> Forward { get; }

        public Indexer<T2, T1> Reverse { get; }

        public Map()
        {
            Forward = new Indexer<T1, T2>(_forward);
            Reverse = new Indexer<T2, T1>(_reverse);
        }

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public bool ContainsLeft(T1 t1)
        {
            return _forward.ContainsKey(t1);
        }

        public bool ContainsRight(T2 t2)
        {
            return _reverse.ContainsKey(t2);
        }
    }
}