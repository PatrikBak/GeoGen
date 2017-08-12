using GeoGen.Generator.Constructor;
using GeoGen.Generator.Container;
using GeoGen.Generator.Handler;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a facade service that <see cref="Generator"/> needs to run. 
    /// </summary>
    internal interface IGeneratorContext
    {
        /// <summary>
        /// Gets the configuration container.
        /// </summary>
        IConfigurationContainer ConfigurationContainer { get; }

        /// <summary>
        /// Gets the configuration handler.
        /// </summary>
        IConfigurationsHandler ConfigurationsHandler { get; }

        /// <summary>
        /// Gets the configuration constructer.
        /// </summary>
        IConfigurationConstructor ConfigurationConstructor { get; }
    }
}