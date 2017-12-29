using GeoGen.Utilities.Combinations;
using GeoGen.Utilities.Subsets;
using GeoGen.Utilities.Variations;

namespace GeoGen.Core
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
