using System;
using System.Diagnostics;

namespace GeoGen.Core.Utilities
{
    public class DebugUtilities
    {
        [Conditional("DEBUG")]
        public static void DebugCode(Action action)
        {
            action?.Invoke();
        }
    }
}