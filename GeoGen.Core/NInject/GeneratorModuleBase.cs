using GeoGen.Core.Utilities.Combinations;
using GeoGen.Core.Utilities.SubsetsProviding;
using GeoGen.Core.Utilities.VariationsProviding;
using Ninject.Modules;

namespace GeoGen.Core.NInject
{
    /// <summary>
    /// A NInject module that binds things from the Core module.
    /// </summary>
    public class CoreModule : NinjectModule
    {
        /// <summary>
        /// Loads all bindings.
        /// </summary>
        public override void Load()
        {
            Bind<ICombinator>().To<Combinator>().InSingletonScope();
            Bind<IVariationsProvider>().To<VariationsProvider>().InSingletonScope();
            Bind<ISubsetsProvider>().To<SubsetsProvider>().InSingletonScope();
        }
    }
}