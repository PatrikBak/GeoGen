using GeoGen.Core;

namespace GeoGen.Utilities
{
    /// <summary>
    /// A NInject module that binds things from the Utilities module.
    /// </summary>
    public class UtilitiesModule : BaseModule
    {
        /// <summary>
        /// Loads all bindings.
        /// </summary>
        public override void Load()
        {
            BindInSingletonScope<ICombinator, Combinator>();
            BindInSingletonScope<IVariationsProvider, VariationsProvider>();
            BindInSingletonScope<ISubsetsProvider, SubsetsProvider>();
        }
    }
}
