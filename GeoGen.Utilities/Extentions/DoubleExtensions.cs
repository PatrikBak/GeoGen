namespace GeoGen.Utilities
{
    public static class DoubleExtensions
    {
        public static double AdjustPrecision(this double d, int digits)
        {
            // TODO: Not sure if this will work for both normalized and denormalized doubles. Needs more research.
            var shift = 53 - digits; // IEEE 754 doubles have 53 bits of significand, but one bit is "implied" and not stored.
            ulong significandMask = (0xffffffffffffffffUL >> shift) << shift;
            var local_d = d;
            unsafe
            {
                // double -> fixed point (sorta)
                ulong toLong = *(ulong*)(&local_d);
                // mask off your least-sig bits
                var modLong = toLong & significandMask;
                // fixed point -> float (sorta)
                local_d = *(double*)(&modLong);
            }
            return local_d;
        }

        //private static readonly double[] PowersOfTwoPlusOne;

        //static DoubleExtensions()
        //{
        //    PowersOfTwoPlusOne = new double[54];

        //    for (var i = 0; i < PowersOfTwoPlusOne.Length; i++)
        //    {
        //        if (i == 0)
        //            PowersOfTwoPlusOne[i] = 1;
        //        else
        //            PowersOfTwoPlusOne[i] = (1L << i) + 1L;
        //    }
        //}

        //public static double AdjustPrecisionSafely(this double d, int digits)
        //{
        //    var t = d * PowersOfTwoPlusOne[53 - digits];
        //    var adjusted = t - (t - d);
        //    return adjusted;
        //}
    }
}
