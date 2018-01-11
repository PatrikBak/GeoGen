using GeoGen.Core.Generator;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a service that takes care of resolving a given <see cref="ConstructorOutput"/>,
    /// which means identifying the objects and deciding whether they are correct. It also handles
    /// this work for a given initial configuration.
    /// </summary>
    internal interface IConfigurationsResolver
    {
        /// <summary>
        /// Resolves a given constructor output.
        /// </summary>
        /// <param name="output">The constructor output.</param>
        /// <returns>true, if the output was resolved as correct, false otherwise</returns>
        bool ResolveNewOutput(ConstructorOutput output);

        /// <summary>
        /// Resolves a given initial configuration. If the configuration
        /// is resolved as incorrect, then an <see cref="InitializationException"/>
        /// will be thrown.
        /// </summary>
        /// <param name="configuration">The initial configuration.</param>
        void ResolveInitialConfiguration(ConfigurationWrapper configuration);
    }
}