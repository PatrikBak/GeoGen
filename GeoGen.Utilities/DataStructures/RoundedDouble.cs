using System;
using System.Globalization;

namespace GeoGen.Utilities
{
    public class RoundedDouble
    {
        public const int DoubleRoundingPrecision = 6;

        public static readonly RoundedDouble Zero = new RoundedDouble(0);

        public double OriginalValue { get; }

        public double RoundedValue { get; }

        private readonly Lazy<int> _hashCode;

        public static int c = 0;

        public RoundedDouble(double originalValue)
        {
            c++;
            OriginalValue = originalValue;
            //var roundedValue = originalValue.AdjustPrecision(DoubleRoundingPrecision);
            var roundedValue = Math.Round(originalValue, DoubleRoundingPrecision);
            RoundedValue = roundedValue;
            _hashCode = new Lazy<int>(() => roundedValue.GetHashCode());
        }

        public static explicit operator RoundedDouble(double value)
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
            if (obj is null)
                return false;

            return obj is RoundedDouble d && Equals(d);
        }

        public override int GetHashCode()
        {
            //return _hashCode.Value;
            return RoundedValue.GetHashCode();
        }

        public override string ToString()
        {
            return RoundedValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}