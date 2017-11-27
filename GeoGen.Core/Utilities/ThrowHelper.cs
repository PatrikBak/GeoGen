using System;

namespace GeoGen.Core.Utilities
{
    public static class ThrowHelper
    {
        public static void ThrowExceptionIfNotTrue(bool condition)
        {
            if (!condition)
                throw new Exception();
        }
    }
}