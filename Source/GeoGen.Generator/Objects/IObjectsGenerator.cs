using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a service that generates all possible <see cref="ConstructorOutput"/>s
    /// for a given configuration.
    /// </summary>
    public interface IObjectsGenerator
    {
        /// <summary>
        /// Constructs all possible construction outputs that can be added
        /// constructed from the objects of a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration which objects should be used.</param>
        /// <returns>The enumerable of all possible construction outputs.</returns>
        IEnumerable<List<ConstructedConfigurationObject>> ConstructPossibleObjects(Configuration configuration);
    }
}