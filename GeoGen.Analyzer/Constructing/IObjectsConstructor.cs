using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a general constructor of <see cref="ConstructedConfigurationObject"/>s.
    /// </summary>
    internal interface IObjectsConstructor
    {
        /// <summary>
        /// Constructs a given list of constructed configurations objects. This objects 
        /// should be the result of the same construction.
        /// </summary>
        /// <param name="constructedObjects">The constructed objects list.</param>
        /// <returns>The constructor output.</returns>
        ConstructorOutput Construct(List<ConstructedConfigurationObject> constructedObjects);
    }
}