using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Constructing
{
    /// <summary>
    /// Represent a simple constructor of loose configuration objects.
    ///
    /// TODO: Consider implementing this with some conditions (such as points should not be collinear)
    /// 
    /// </summary>
    internal interface ILooseObjectsConstructor
    {
        /// <summary>
        /// Constructs given loose objects.
        /// </summary>
        /// <param name="looseObjects">The loose objects.</param>
        /// <returns>The list of analytical objects.</returns>
        List<IAnalyticalObject> Construct(IEnumerable<LooseConfigurationObject> looseObjects);
    }
}