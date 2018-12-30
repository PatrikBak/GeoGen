using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represent a generator of all possible mutually distinct <see cref="Arguments"/> from given objects 
    /// passed as <see cref="ConfigurationObjectsMap"/> matching a given <see cref="Signature"/>.
    /// </summary>
    public interface IArgumentsGenerator
    {
        /// <summary>
        /// Generates all possible arguments, that match a given signature, 
        /// using the configurations objects from a given configuration objects map.
        /// </summary>
        /// <param name="availableObjects">The configuration objects map of the objects that can be used in generated arguments.</param>
        /// <param name="construction">The signature that should be matched.</param>
        /// <returns>The generated arguments enumerable.</returns>
        IEnumerable<Arguments> GenerateArguments(ConfigurationObjectsMap availableObjects, Signature signature);
    }
}