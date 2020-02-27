namespace GeoGen.ConfigurationGenerationLauncher
{
    /// <summary>
    /// Represents which configurations are counted in while running <see cref="GenerationOnlyProblemGenerationRunner"/>.
    /// </summary>
    public enum CountingMode
    {
        /// <summary>
        /// Only configurations of the last iteration are counted in.
        /// </summary>
        LastIteration,

        /// <summary>
        /// All configurations are counted in.
        /// </summary>
        All
    }
}
