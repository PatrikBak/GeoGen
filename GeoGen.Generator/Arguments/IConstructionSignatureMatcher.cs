using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Core.Utilities;

namespace GeoGen.Generator.ConstructingObjects.Arguments.SignatureMatching
{
    /// <summary>
    /// Represents a converter between lists of <see cref="ConstructionParameter"/>s and 
    /// lists of <see cref="ConstructionArgument"/>s. It pulls objects from a 
    /// <see cref="ConfigurationObjectsMap"/>, since they're already sorted by the 
    /// type in the map.
    /// </summary>
    internal interface IConstructionSignatureMatcher
    {
        /// <summary>
        /// Constructs construction arguments that match the given construction parameters. It must be
        /// possible to perform the construction, otherwise a <see cref="GeneratorException"/> is thrown.
        /// The objects are given in a configuration objects map.
        /// </summary>
        /// <param name="parameters">The parameters list.</param>
        /// <param name="map">The configuration objects map.</param>
        /// <returns>The created arguments.</returns>
        List<ConstructionArgument> Match(IReadOnlyList<ConstructionParameter> parameters, ConfigurationObjectsMap map);
    }
}