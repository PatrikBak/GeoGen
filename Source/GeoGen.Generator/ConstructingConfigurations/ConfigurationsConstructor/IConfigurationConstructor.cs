using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a constructor of <see cref="ConfigurationWrapper"/>.
    /// </summary>
    internal interface IConfigurationConstructor
    {
        /// <summary>
        /// Constructs a configuration wrapper from a new configuration to be wrapped 
        /// and the construction that was extended.
        /// </summary>
        /// <param name="newConfiguration">The configuration to be wrapped.</param>
        /// <param name="oldConfiguration">The old configuration that was extended.</param>
        /// <returns>The wrapper of the configuration.</returns>
        ConfigurationWrapper ConstructWrapper(Configuration newConfiguration, ConfigurationWrapper oldConfiguration);

        /// <summary>
        /// Constructs a configuration wrapper from a given initial configuration.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        /// <returns>The wrapper of the configuration.</returns>
        ConfigurationWrapper ConstructInitialWrapper(Configuration initialConfiguration);
    }
}