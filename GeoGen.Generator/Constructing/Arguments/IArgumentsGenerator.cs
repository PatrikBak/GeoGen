using GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer;
using GeoGen.Generator.Constructing.Arguments.Container;

namespace GeoGen.Generator.Constructing.Arguments
{
    /// <summary>
    /// Represent a generator of all possible mutally distinct construction arguments, for
    /// given <see cref="ConfigurationWrapper"/> and <see cref="ConstructionWrapper"/>.
    /// </summary>
    internal interface IArgumentsGenerator
    {
        /// <summary>
        /// Generates a container of all possible distinct arguments that can be passed to 
        /// a given construction, using object from a given configuration.
        /// </summary>
        /// <param name="configuration">The wrapper cofiguration.</param>
        /// <param name="construction">The wrapped construction.</param>
        /// <returns>The container of resulting arguments.</returns>
        IArgumentsContainer GenerateArguments(ConfigurationWrapper configuration, ConstructionWrapper construction);
    }
}