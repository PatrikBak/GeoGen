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
        /// Creates a new contextual container filled with given configuration objects.
        /// </summary>
        /// <param name="objects">The objects.</param>
        /// <returns>The container.</returns>
        IContextualContainer Create(IEnumerable<ConfigurationObject> objects);
    }
}
