using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a service that takes care of resolving a given <see cref="Configuration"/>,
    /// which means identifying the objects and deciding whether they are correct. It also handles
    /// this work for a given initial configuration.
    /// </summary>
    public interface IConfigurationsResolver
    {
        /// <summary>
        /// Resolves a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>true, if the configuration was resolved as correct, false otherwise</returns>
        bool ResolveNewConfiguration(Configuration configuration);

        /// <summary>
        /// Resolves a given initial configuration. If the configuration
        /// is resolved as incorrect, then an <see cref="InitializationException"/>
        /// will be thrown.
        /// </summary>
        /// <param name="configuration">The initial configuration.</param>
        void ResolveInitialConfiguration(Configuration configuration);
    }
}