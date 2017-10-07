using System;

namespace GeoGen.Core.Utilities
{
    public static class DoubleExtensions
    {
        public static bool AlmostEquals(this double value1, double value2, int units = 10)
        {
            var lValue1 = BitConverter.DoubleToInt64Bits(value1);
            var lValue2 = BitConverter.DoubleToInt64Bits(value2);

            // If the signs are different, return false except for +0 and -0.
            if (lValue1 >> 63 != lValue2 >> 63)
            {
                return value1 == value2;
            }

            var diff = Math.Abs(lValue1 - lValue2);

            return diff <= units;
        }
    }
}