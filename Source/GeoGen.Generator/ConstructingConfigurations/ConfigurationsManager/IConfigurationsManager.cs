using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a manager for the gradual generation of configurations. It works like this:
    /// We have a current layer of configurations and then from each one we generate new configurations.
    /// Then we merge the new configurations and take them as the next layer. This manager
    /// is responsible for handling this. 
    /// </summary>
    internal interface IConfigurationsManager
    {
        /// <summary>
        /// Gets the current layer of unprocessed configurations.
        /// </summary>
        List<ConfigurationWrapper> CurrentLayer { get; }

        /// <summary>
        /// Processes a new layer of constructors outputs and lazily returns the configurations.
        /// </summary>
        /// <param name="output">The outputs for the new layer.</param>
        /// <returns>The configuration wrappers.</returns>
        IEnumerable<ConfigurationWrapper> AddLayer(IEnumerable<ConstructorOutput> output);
    }
}