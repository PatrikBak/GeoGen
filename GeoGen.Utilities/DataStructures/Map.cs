using System.Collections.Generic;

namespace GeoGen.Utilities
{
    public sealed class Map<T1, T2>
    {
        private readonly Dictionary<T1, T2> _leftToRight = new Dictionary<T1, T2>();

        private readonly Dictionary<T2, T1> _rightToLeft = new Dictionary<T2, T1>();

        public void Add(T1 t1, T2 t2)
        {
            _leftToRight.Add(t1, t2);
            _rightToLeft.Add(t2, t1);
        }

        public T1 GetLeft(T2 key)
        {
            return _rightToLeft[key];
        }

        public T2 GetRight(T1 key)
        {
            return _leftToRight[key];
        }

        public bool ContainsLeft(T1 t1)
        {
            return _leftToRight.ContainsKey(t1);
        }

        public bool ContainsRight(T2 t2)
        {
            return _rightToLeft.ContainsKey(t2);
        }
    }
}