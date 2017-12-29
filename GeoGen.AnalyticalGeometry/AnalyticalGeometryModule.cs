using GeoGen.AnalyticalGeometry.RandomObjects;
using GeoGen.Core;

namespace GeoGen.AnalyticalGeometry
{
    /// <summary>
    /// A NInject module that binds things from the Analytical Geometry module.
    /// </summary>
    public class AnalyticalGeometryModule : BaseModule
    {
        /// <summary>
        /// Loads all bindings.
        /// </summary>
        public override void Load()
        {
            BindInSingletonScope<IRandomObjectsProvider, RandomObjectsProvider>();
            BindInSingletonScope<IAnalyticalHelper, AnalyticalHelper>();
            BindInSingletonScope<IRandomnessProvider, RandomnessProvider>();
        }
    }
}