using Ninject.Modules;

namespace GeoGen.Utilities
{
    /// <summary>
    /// A NInject module that binds things from the Utilities module.
    /// </summary>
    public class UtilitiesModule : NinjectModule
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
