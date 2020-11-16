using GeoGen.Core;

namespace GeoGen.ProblemGenerator
{
    /// <summary>
    /// Specifies what types of configurations should be generated, with respect to symmetry.
    /// </summary>
    public enum SymmetryGenerationMode
    {
        /// <summary>
        /// Generates only symmetric configurations, i.e. any configuration for which there exists a symmetry
        /// mapping (<see cref="LooseObjectHolder.GetSymmetricMappings"/>) under which it stays the same.
        /// </summary>
        GenerateOnlySymmetric,

        /// <summary>
        /// Generates only fully symmetric configurations, i.e. any configuration for which there all symmetry
        /// mapping (<see cref="LooseObjectHolder.GetSymmetricMappings"/>) keep it the same.
        /// </summary>
        GenerateOnlyFullySymmetric,

        /// <summary>
        /// Generates both symmetric and asymmetric configuration.
        /// </summary>
        GenerateBothSymmetricAndAsymmetric
    }
}