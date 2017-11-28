using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;

namespace GeoGen.Generator.ConstructingConfigurations
{
    /// <summary>
    /// An internal wrapper sealed class for a <see cref="Core.Configurations.Configuration"/>, containing some
    /// additional data regarding the configuration. 
    /// </summary>
    internal sealed class ConfigurationWrapper
    {
        /// <summary>
        /// Gets or sets the configuration object that this sealed class wraps.
        /// </summary>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the precalculated configuration objects map. This map is
        /// useful during the objects construction process, since we need to have
        /// objects distinguished by their type.
        /// </summary>
        public ConfigurationObjectsMap ConfigurationObjectsMap { get; set; }
    }
}