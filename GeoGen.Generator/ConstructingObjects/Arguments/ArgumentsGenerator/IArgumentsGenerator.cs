namespace GeoGen.Generator
{
    /// <summary>
    /// Represent a generator of all possible mutually distinct construction arguments, for
    /// given <see cref="ConfigurationWrapper"/> and <see cref="ConstructionWrapper"/>.
    /// </summary>
    internal interface IArgumentsGenerator
    {
        /// <summary>
        /// Generates a container of all possible distinct arguments that can be passed to 
        /// a given construction, using objects from a given configuration.
        /// </summary>
        /// <param name="configuration">The wrapped configuration.</param>
        /// <param name="construction">The wrapped construction.</param>
        /// <returns>The container of resulting arguments.</returns>
        IArgumentsListContainer GenerateArguments(ConfigurationWrapper configuration, ConstructionWrapper construction);
    }
}