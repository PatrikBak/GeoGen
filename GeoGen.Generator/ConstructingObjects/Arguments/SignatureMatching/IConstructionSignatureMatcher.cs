using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;
using GeoGen.Core.Utilities;

namespace GeoGen.Generator.ConstructingObjects.Arguments.SignatureMatching
{
    /// <summary>
    /// Represents a converter between <see cref="ConstructionParameter"/>s and 
    /// <see cref="ConstructionArgument"/>s. 
    /// </summary>
    internal interface IConstructionSignatureMatcher
    {
        /// <summary>
        /// Constructs construction arguments that match the given construction parameters. It must be
        /// possible to perform the construction, otherwise a <see cref="GeneratorException"/> is thrown.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The created arguments.</returns>
        List<ConstructionArgument> Match(IReadOnlyList<ConstructionParameter> parameters, ConfigurationObjectsMap map);
    }
}