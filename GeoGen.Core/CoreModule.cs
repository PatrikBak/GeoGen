using GeoGen.Utilities;

namespace GeoGen.Core.NInject
{
    public class CoreModule : BaseModule
    {
        public override void Load()
        {
            BindInSingletonScope<ICombinator, Combinator>();
            BindInSingletonScope<IVariationsProvider, VariationsProvider>();
            BindInSingletonScope<ISubsetsProvider, SubsetsProvider>();
        }
    }
}
