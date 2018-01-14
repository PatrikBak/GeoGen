using System;

namespace GeoGen.AnalyticalGeometry
{
    /// <summary>
    /// A base class for <see cref="AnalyticalObject"/>s that takes care of 
    /// caching the hash code, implementing GetHashCode, Equals and the
    /// equality operators.
    /// </summary>
    /// <typeparam name="T">The type of this object.</typeparam>
    public abstract class AnalyticalObjectBase<T> : AnalyticalObject where T : AnalyticalObjectBase<T>
    {
        #region Private fields

        /// <summary>
        /// The initializer of the hash code of this object.
        /// </summary>
        private readonly Lazy<int> _hashCodeInitializer;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected AnalyticalObjectBase()
        {
            // Initialize the hash code initializer
            _hashCodeInitializer = new Lazy<int>(CalculateHashCode);
        }

        #endregion

        #region Protected abstract methods

        /// <summary>
        /// Calculates the hash code of the object. This method is called once per
        /// object, unlike GetHashCode, which will reuse the result of this method.
        /// </summary>
        /// <returns>The hash code.</returns>
        protected abstract int CalculateHashCode();

        /// <summary>
        /// Returns if a given analytical object is equal to this one.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>true, if the objects are equal, false otherwise.</returns>
        protected abstract bool IsEqualTo(T other);

        #endregion

        #region HashCode and Equals

        public override int GetHashCode()
        {
            return _hashCodeInitializer.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (!(obj is T other))
                return false;

            return IsEqualTo(other);
        }

        #endregion

        #region Equality operators

        public static bool operator ==(AnalyticalObjectBase<T> left, AnalyticalObjectBase<T> right)
        {
            return left?.Equals(right) ?? right is null;
        }

        public static bool operator !=(AnalyticalObjectBase<T> left, AnalyticalObjectBase<T> right)
        {
            return !(left == right);
        }

        #endregion
    }
}