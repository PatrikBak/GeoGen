using System;
using System.Globalization;

namespace GeoGen.Utilities
{
    public struct RoundedDouble
    {
        public const int DoubleRoundingPrecision = 6;

        public double OriginalValue { get; }

        public double RoundedValue { get; }

        public RoundedDouble(double originalValue)
        {
            OriginalValue = originalValue;
            RoundedValue = Math.Round(originalValue, DoubleRoundingPrecision);
        }

        public static implicit operator RoundedDouble(double value)
        {
            return new RoundedDouble(value);
        }

        public static implicit operator double(RoundedDouble value)
        {
            return value.OriginalValue;
        }

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
            return !left.Equals(right);
        }

        public bool Equals(RoundedDouble other)
        {
            return RoundedValue.Equals(other.RoundedValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is RoundedDouble && Equals((RoundedDouble) obj);
        }

        public override int GetHashCode()
        {
            return RoundedValue.GetHashCode();
        }

        public override string ToString()
        {
            return RoundedValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}