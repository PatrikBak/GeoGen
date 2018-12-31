using System;
using System.Globalization;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// Represents a <see cref="double"/> structure that holds its rounded value which 
    /// is used while comparing two rounded doubles.
    /// </summary>
    public struct RoundedDouble
    {
        #region Public constants

        /// <summary>
        /// The number of decimal places to which the double is rounded.
        /// </summary>
        public const int RoundingPrecision = 10;

        #endregion

        #region Public static fields

        /// <summary>
        /// The constant representing the value 0.
        /// </summary>
        public static readonly RoundedDouble Zero = new RoundedDouble(0);

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the original value of the double number.
        /// </summary>
        public double OriginalValue { get; }

        /// <summary>
        /// Gets the rounded value of the double number.
        /// </summary>
        public double RoundedValue { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="originalValue">The original value.</param>
        public RoundedDouble(double originalValue)
        {
            OriginalValue = originalValue;
            RoundedValue = Math.Round(originalValue, RoundingPrecision);
        }

        #endregion

        #region Conversion operators

        public static explicit operator RoundedDouble(double value)
        {
            return new RoundedDouble(value);
        }

        public static implicit operator double(RoundedDouble value)
        {
            return value.OriginalValue;
        }

        #endregion

        #region Arithmetic operators

        public static double operator +(RoundedDouble double1, RoundedDouble double2)
        {
            return double1.OriginalValue + double2.OriginalValue;
        }

        public static double operator -(RoundedDouble double1, RoundedDouble double2)
        {
            return double1.OriginalValue - double2.OriginalValue;
        }

        public static double operator *(RoundedDouble double1, RoundedDouble double2)
        {
            return double1.OriginalValue * double2.OriginalValue;
        }

        public static double operator /(RoundedDouble double1, RoundedDouble double2)
        {
            return double1.OriginalValue / double2.OriginalValue;
        }

        #endregion

        #region Comparison operators

        public static bool operator >(RoundedDouble double1, RoundedDouble double2)
        {
            return double1.RoundedValue > double2.RoundedValue;
        }

        public static bool operator <(RoundedDouble double1, RoundedDouble double2)
        {
            return double1.RoundedValue < double2.RoundedValue;
        }

        public static bool operator >=(RoundedDouble double1, RoundedDouble double2)
        {
            return double1.RoundedValue >= double2.RoundedValue;
        }

        public static bool operator <=(RoundedDouble double1, RoundedDouble double2)
        {
            return double1.RoundedValue <= double2.RoundedValue;
        }

        public static bool operator ==(RoundedDouble left, RoundedDouble right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RoundedDouble left, RoundedDouble right)
        {
            return !(left == right);
        }

        #endregion

        #region Equals and hash code

        /// <summary>
        /// Finds out if a given rounded double is equal to this one,
        /// by comparing its rounded values.
        /// </summary>
        /// <param name="other">The other double.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public bool Equals(RoundedDouble other)
        {
            return RoundedValue.Equals(other.RoundedValue);
        }

        /// <summary>
        /// Finds out if a passed object is equal to this one.
        /// </summary>
        /// <param name="otherObject">The passed object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public override bool Equals(object otherObject)
        {
            if (otherObject is null)
                return false;

            return otherObject is RoundedDouble d && Equals(d);
        }

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => RoundedValue.GetHashCode();

        #endregion

        #region To string

        /// <summary>
        /// Converts a given value to a string. 
        /// NOTE: This method id used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the value.</returns>
        public override string ToString() => OriginalValue.ToString(CultureInfo.InvariantCulture);

        #endregion
    }
}