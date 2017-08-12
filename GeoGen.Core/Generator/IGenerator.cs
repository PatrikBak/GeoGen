using System.Collections.Generic;

namespace GeoGen.Core.Generator
{
    /// <summary>
    /// Represents the generator service, the key inferface of the Generator module.
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Starts the generation proccess and lazily return the output.
        /// </summary>
        /// <returns>The generator output enumerable.</returns>
        IEnumerable<GeneratorOutput> Generate();
    }
}