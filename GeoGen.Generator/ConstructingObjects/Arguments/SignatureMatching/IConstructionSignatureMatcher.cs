using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Generator.ConstructingObjects.Arguments.SignatureMatching
{
    /// <summary>
    /// Represents a converter between <see cref="ConstructionParameter"/>s and 
    /// <see cref="ConstructionArgument"/>s. 
    /// </summary>
    internal interface IConstructionSignatureMatcher
    {
        /// <summary>
        /// Initializes the signature matcher with objects represented as a map between objects types
        /// and all objects of that type.
        /// </summary>
        /// <param name="objectTypeToObjects">The objects dictionary.</param>
        void Initialize(IReadOnlyDictionary<ConfigurationObjectType, List<ConfigurationObject>> objectTypeToObjects);

        /// <summary>
        /// Constructs construction arguments that match the given construction parameters. It must be
        /// possible to perform the construction, otherwise a <see cref="GeneratorException"/> is thrown.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The created arguments.</returns>
        IReadOnlyList<ConstructionArgument> Match(IReadOnlyList<ConstructionParameter> parameters);
    }
}