using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A factory for creating <see cref="IContextualContainer"/> from given objects.
    /// </summary>
    internal interface IContextualContainerFactory
    {
        /// <summary>
        /// Creates a new contextual container that represents a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The container.</returns>
        IContextualContainer Create(Configuration configuration);
    }
}
