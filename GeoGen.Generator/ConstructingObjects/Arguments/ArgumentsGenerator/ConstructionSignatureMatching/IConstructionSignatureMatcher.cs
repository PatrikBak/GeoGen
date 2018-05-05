using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a converter of a list of <see cref="ConstructionParameter"/>s to <see cref="Arguments"/>.
    /// </summary>
    internal interface IConstructionSignatureMatcher
    {
        /// <summary>
        /// Constructs construction arguments that match the given construction parameters. 
        /// The objects that are actually passed to as the arguments are given in a 
        /// configuration objects map.
        /// </summary>
        /// <param name="parameters">The parameters list.</param>
        /// <param name="map">The configuration objects map.</param>
        /// <returns>The created arguments.</returns>
        Arguments Match(IReadOnlyList<ConstructionParameter> parameters, ConfigurationObjectsMap map);
    }
}