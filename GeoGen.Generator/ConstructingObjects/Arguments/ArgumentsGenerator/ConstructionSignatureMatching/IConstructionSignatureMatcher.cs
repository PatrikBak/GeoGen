using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Core.Generator;
using GeoGen.Core.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a converter of a list of <see cref="ConstructionParameter"/>s to a 
    /// list of <see cref="ConstructionArgument"/>s. 
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
        List<ConstructionArgument> Match(IReadOnlyList<ConstructionParameter> parameters, ConfigurationObjectsMap map);
    }
}