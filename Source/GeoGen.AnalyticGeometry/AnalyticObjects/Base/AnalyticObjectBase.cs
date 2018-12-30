using System;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// A base class for <see cref="AnalyticObject"/>s that takes care of 
    /// caching the hash code, implementing GetHashCode, Equals and the
    /// equality operators.
    /// </summary>
    /// <typeparam name="T">The type of this object.</typeparam>
    public abstract class AnalyticObjectBase<T> : AnalyticObject where T : AnalyticObjectBase<T>
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
        protected AnalyticObjectBase()
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
        /// Returns if a given analytic object is equal to this one.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns>true, if the objects are equal, false otherwise.</returns>
        protected abstract bool IsEqualTo(T other);

        #endregion

        #region HashCode and Equals

        /// <summary>
        /// Finds out if a passed object is equal to this one.
        /// </summary>
        /// <param name="otherObject">The passed object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public override bool Equals(object otherObject)
        {
            if (otherObject is null)
                return false;

            if (!(otherObject is T other))
                return false;

            return IsEqualTo(other);
        }

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => _hashCodeInitializer.Value;

        #endregion

        #region Equality operators

        public static bool operator ==(AnalyticObjectBase<T> left, AnalyticObjectBase<T> right)
        {
            return left?.Equals(right) ?? right is null;
        }

        public static bool operator !=(AnalyticObjectBase<T> left, AnalyticObjectBase<T> right)
        {
            return !(left == right);
        }

        #endregion
    }
}