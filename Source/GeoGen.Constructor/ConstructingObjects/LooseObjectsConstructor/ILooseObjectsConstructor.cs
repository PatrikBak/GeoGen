using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represent a constructor of <see cref="LooseObjectsHolder"/>.
    /// </summary>
    public interface ILooseObjectsConstructor
    {
        /// <summary>
        /// Constructs the objects of a given loose objects holder.
        /// </summary>
        /// <param name="looseObjectsHolder">The loose objects holder whose objects should be constructed.</param>
        /// <returns>Analytic versions of particular loose objects of the holder.</returns>
        List<IAnalyticObject> Construct(LooseObjectsHolder looseObjectsHolder);
    }
}