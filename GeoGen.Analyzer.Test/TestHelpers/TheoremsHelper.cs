using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Test.TestHelpers
{
    internal static class TheoremsHelper
    {
        public static bool ContainsTheoremWithObjects(this List<Theorem> theorems, params ConfigurationObject[] objects)
        {
            return theorems.Any
            (
                theorem =>
                {
                    return theorem
                            .InvolvedObjects
                            .SelectMany(o => o.InternalObjects)
                            .ToSet()
                            .SetEquals(objects.ToSet());
                }
            );
        }
    }
}