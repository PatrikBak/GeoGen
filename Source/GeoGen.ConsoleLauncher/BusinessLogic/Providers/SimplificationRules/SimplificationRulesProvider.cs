using GeoGen.TheoremSimplifier;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoGen.ConsoleLauncher
{
    public class SimplificationRulesProvider : ISimplificationRulesProvider
    {
        public Task<IReadOnlyList<SimplificationRule>> GetSimplificationRulesAsync()
        {
            return Task.FromResult((IReadOnlyList<SimplificationRule>)new List<SimplificationRule>());
        }
    }
}
