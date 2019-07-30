using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a service that gets <see cref="LoadedGeneratorInput"/>s for the algorithm.
    /// </summary>
    public interface IGeneratorInputsProvider
    {
        /// <summary>
        /// Gets generator inputs.
        /// </summary>
        /// <returns>The generator inputs.</returns>
        Task<List<LoadedGeneratorInput>> GetGeneratorInputsAsync();
    }
}
