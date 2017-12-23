using GeoGen.AnalyticalGeometry.RandomObjects;
using GeoGen.Core.NInject;
using Ninject.Modules;

namespace GeoGen.AnalyticalGeometry.Ninject
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