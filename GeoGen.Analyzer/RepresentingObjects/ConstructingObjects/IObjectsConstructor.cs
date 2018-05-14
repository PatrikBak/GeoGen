using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a general constructor of lists <see cref="ConstructedConfigurationObject"/>s
    /// that are created with the same <see cref="Construction"/> and the same <see cref="ConstructionArgument"/>s.
    /// </summary>
    internal interface IObjectsConstructor
    {
        /// <summary>
        /// Constructs a given list of constructed configurations objects. These objects 
        /// should be the result of the same construction.
        /// </summary>
        /// <param name="constructedObjects">The constructed objects list.</param>
        /// <returns>The constructor output.</returns>
        ConstructorOutput Construct(IReadOnlyList<ConstructedConfigurationObject> constructedObjects);
    }
}