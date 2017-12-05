using GeoGen.AnalyticalGeometry.RandomObjects;
using Ninject.Modules;

namespace GeoGen.AnalyticalGeometry.Ninject
{
    /// <summary>
    /// A NInject module that binds things from the Analytical Geometry module.
    /// </summary>
    public class AnalyticalGeometryModule : NinjectModule
    {
        /// <summary>
        /// Loads all bindings.
        /// </summary>
        public override void Load()
        {
            Bind<IRandomObjectsProvider>().To<RandomObjectsProvider>().InSingletonScope();
            Bind<IAnalyticalHelper>().To<AnalyticalHelper>().InSingletonScope();
            Bind<IRandomnessProvider>().To<RandomnessProvider>().InSingletonScope();
        }
    }
}