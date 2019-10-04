using GeoGen.Core;
using GeoGen.Generator;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a parser for <see cref="GeneratorInput"/> and <see cref="Theorem"/>s.
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Parses a given content to a generator input.
        /// </summary>
        /// <param name="content">The content of an input file.</param>
        /// <returns>The parsed generator input.</returns>
        GeneratorInput ParseInput(string content);

        /// <summary>
        /// Parses a given content to the list of theorems and the configuration where the theorems hold.
        /// </summary>
        /// <param name="content">The content of a file containing template theorems.</param>
        /// <returns>The parsed theorems with configuration.</returns>
        (List<Theorem> theorems, Configuration configuration) ParseTheoremsAndConfiguration(string content);
    }
}
