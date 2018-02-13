using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represent a constructor of loose configuration objects.
    /// </summary>
    internal interface ILooseObjectsConstructor
    {
        /// <summary>
        /// Constructs given loose objects.
        /// </summary>
        /// <param name="looseObjects">The loose objects.</param>
        /// <returns>The list of analytical objects.</returns>
        List<AnalyticalObject> Construct(LooseObjectsHolder looseObjects);
    }
}