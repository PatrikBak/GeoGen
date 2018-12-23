using System.Collections.Generic;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represent a constructor of loose configuration objects.
    /// </summary>
    public interface ILooseObjectsConstructor
    {
        /// <summary>
        /// Constructs given loose objects.
        /// </summary>
        /// <param name="looseObjects">The loose objects.</param>
        /// <returns>The list of analytic objects.</returns>
        List<AnalyticObject> Construct(LooseObjectsHolder looseObjects);
    }
}