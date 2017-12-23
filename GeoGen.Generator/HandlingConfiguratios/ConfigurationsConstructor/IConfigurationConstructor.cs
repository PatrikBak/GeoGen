using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a constructor of configuration wrappers from a given
    /// initial <see cref="Configuration"/>, or from a <see cref="ConstructorOutput"/>.
    /// </summary>
    internal interface IConfigurationConstructor
    {
        /// <summary>
        /// Constructs a configuration wrapper from a given constructor output.
        /// </summary>
        /// <param name="constructorOutput">The constructor output.</param>
        /// <returns>The wrapper of the new configuration.</returns>
        ConfigurationWrapper ConstructWrapper(ConstructorOutput constructorOutput);

        /// <summary>
        /// Constructs a configuration wrapper from a given configuration (meant
        /// to be as initial).
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        /// <returns>The wrapper of the initial configuration.</returns>
        ConfigurationWrapper ConstructWrapper(Configuration initialConfiguration);
    }
}