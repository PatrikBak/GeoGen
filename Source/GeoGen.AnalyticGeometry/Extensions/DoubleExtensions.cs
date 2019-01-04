namespace GeoGen.AnalyticGeometry
{
    public static class DoubleExtensions
    {
        public static unsafe double Rounded(this double number, int precision = 25)
        {
            var bits = *(long*) &number;

            var shift = ((bits >> 52) & ((1L << 11) - 1)) - 1023 + precision;

            if (shift < 0)
                return 0;

            var importantBits = (int) (12 + shift);

            if (importantBits >= 63)
                return number;

            bits = bits & (((1L << importantBits) - 1) << (64 - importantBits));

            if (((bits >> (64 - importantBits)) & 1) == 1)
                bits += 1L << (64 - importantBits);

            return *(double*) &bits;
        }

        public static double Squared(this double number) => number * number;
    }
}