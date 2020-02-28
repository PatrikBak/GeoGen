using System.Globalization;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="double"/>.
    /// </summary>
    public static class DoubleExtensions
    {
        /// <summary>
        /// Rounds a given number in binary.
        /// </summary>
        /// <param name="number">The number to be rounded.</param>
        /// <param name="precision">The number of binary places to which we want to round the number.</param>
        /// <returns>The rounded value of the number.</returns>
        public static unsafe double Rounded(this double number, int precision = 25)
        {
            // Get the IEEE 754 bits of the number. 
            var bits = *(long*)&number;

            // The number is represented in the form:
            // 
            // (-1)^(first bit) . 2^(next 11 bits - 1023) . (1 + remaining 52 bits)
            //
            // To get the exponent we first get rid of the last 52 bits with >> 52, then take
            // only the last 11 bits using & with the number 111..111 having 11 ones (which is 
            // essentially equal to 2^11 - 1, which can be calculated with (1 << 11) - 1, then
            // we get the final exponent by subtracting 1023. 
            // 
            // Before describing how the rounding works, let's see some decimal examples:
            //
            // Round 4.2666 . 10^2 to 1 decimal place: 1+2 = 3, the third digit after the 
            // decimal point is 6, so the result is 4.267 . 10^2. 
            //
            // Round 4.2666 . 10^2 to 4 decimal place: 1+4 = 5, the fifth digit after the 
            // decimal point is 0, so the number doesn't change.
            //
            // Rounded 4.2666 . 10^(-1) to 2 decimal places: -1+2 = 1, the first digit after the
            // decimal point is 2, so the result is 4.2 . 10^(-1). 
            //
            // Rounded 4.2666 . 10^(-1) to 1 decimal place: -1+1 = 0, rounding to 0 places means
            // to cut the decimal part and round the first digit, so the result is 4 . 10^(-1).
            // 
            // Rounded 4.2666 . 10^(-2) to 1 decimal place: -2+1 = -1 < 0, so the result is 0.
            //
            // It works just like this even in binary. In order to find how binary places
            // we're interested about, we just need to add the precision to the exponent.
            var importantPlaces = ((bits >> 52) & ((1L << 11) - 1)) - 1023 + precision;

            // If the number of interesting places is less then 0, then we have the case 
            // where the number is too small and will have many zeros after the floating point,
            // so it will be rounded down to 0.
            if (importantPlaces < 0)
                return 0;

            // If we're interested in more places than we actually store, then we can't do much 
            if (importantPlaces > 52)
                return number;

            // Since the first bit is sign and another 11 ones represent the exponent, 
            // our interesting part starts 1 + 11 = 12 bits further
            var importantBits = (int)(12 + importantPlaces);

            // We create the mask having exactly importantBits ones. If this 
            // number is not 64 then it's 2^importantBits - 1. In the 64 case 
            // we can't use this trick, because 2^64 overflows. Instead, we use 
            // the binary negation of 0, which is exactly what we need in that case
            var mask = importantBits != 64 ? (1L << importantBits) - 1 : ~0L;

            // Now we apply the bit mask to cut all the bits we're not interested.
            // We're interested in the first 'importantBits'so we need to fill the
            // mask with zeros (by shifting to left) so its length is 64
            bits &= mask << (64 - importantBits);

            // Now we check the last bit that we didn't make zero. If it's 1, we need to
            // round up, by adding 1 at that specific place. Getting the bit at the
            // position '64 - importantBits' is done by shifting the interesting bit 
            // to the very right, and then using & with 1, which will cut all the 
            // renaming bits on the left of the last one. Getting 1 at the right place
            // is then simply by using the left shift. We're basically adding the number
            // 2^(64-importantBits) to the final result
            if (((bits >> (64 - importantBits)) & 1) == 1)
                bits += 1L << (64 - importantBits);

            // Finally we can convert the adjusted bits to a double number
            return *(double*)&bits;
        }

        /// <summary>
        /// Calculates the square of a given number, i.e. number * number.
        /// </summary>
        /// <param name="number">The number to be squared.</param>
        /// <returns>The square of the number.</returns>
        public static double Squared(this double number) => number * number;

        /// <summary>
        /// Converts the number to a string with a decimal dot.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <param name="numberOfDecimalPlaces">The number of decimal places that should be in the result.</param>
        /// <returns>The string representation of the number.</returns>
        public static string ToStringWithDecimalDot(this double number, int numberOfDecimalPlaces = 2)
        {
            // Prepare the result by using # to indicate a decimal place and the invariant culture info to ensure a decimal dot
            var result = number.ToString($"0.{new string('#', numberOfDecimalPlaces)}", CultureInfo.InvariantCulture);

            // Sadly, negative 0 gets converted to -0, which is ugly, so let's fix it
            if (result == "-0")
                return "0";

            // In other cases we have the result right away
            return result;
        }
    }
}