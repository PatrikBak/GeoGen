using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Represents a data structure that acts like a <see cref="Dictionary{TKey,TValue}"/>,
    /// but in both ways (i.e. you can get TKey items from TValues).
    /// </summary>
    /// <typeparam name="T1">The value of first items.</typeparam>
    /// <typeparam name="T2">The value of second items.</typeparam>
    public class Map<T1, T2>
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping T1 items to T2 ones.
        /// </summary>
        private readonly Dictionary<T1, T2> _leftToRight = new Dictionary<T1, T2>();

        /// <summary>
        /// The dictionary mapping T2 items to T1 ones.
        /// </summary>
        /// 
        private readonly Dictionary<T2, T1> _rightToLeft = new Dictionary<T2, T1>();

        #endregion

        #region Public methods

        /// <summary>
        /// Adds given items to the map.
        /// </summary>
        /// <param name="t1">The first item.</param>
        /// <param name="t2">The second item.</param>
        public void Add(T1 t1, T2 t2)
        {
            _leftToRight.Add(t1, t2);
            _rightToLeft.Add(t2, t1);
        }

        /// <summary>
        /// Gets the T1 item corresponding to a given T2 key.
        /// </summary>
        /// <param name="key">The T2 key.</param>
        /// <returns>The corresponding T1 item.</returns>
        public T1 GetLeft(T2 key)
        {
            return _rightToLeft[key];
        }

        /// <summary>
        /// Gets the T2 item corresponding to a given T1 key.
        /// </summary>
        /// <param name="key">The T1 key.</param>
        /// <returns>The corresponding T2 item.</returns>
        public T2 GetRight(T1 key)
        {
            return _leftToRight[key];
        }

        /// <summary>
        /// Checks if the map contains a given T1 item.
        /// </summary>
        /// <param name="t1">The T1 item.</param>
        /// <returns>true, if the map contains the item; false otherwise</returns>
        public bool ContainsLeft(T1 t1)
        {
            return _leftToRight.ContainsKey(t1);
        }

        /// <summary>
        /// Checks if the map contains a given T2 item.
        /// </summary>
        /// <param name="t2">The T2 item.</param>
        /// <returns>true, if the map contains the item; false otherwise</returns>
        public bool ContainsRight(T2 t2)
        {
            return _rightToLeft.ContainsKey(t2);
        }

        #endregion
    }
}