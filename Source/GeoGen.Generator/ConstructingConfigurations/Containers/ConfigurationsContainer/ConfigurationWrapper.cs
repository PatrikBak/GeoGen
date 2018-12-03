using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// An internal wrapper class for a <see cref="Configuration"/>, 
    /// containing additional data regarding the configuration that are used
    /// during the generation process.
    /// </summary>
    internal class ConfigurationWrapper
    {
        /// <summary>
        /// Gets or sets the actual configuration that this class wraps.
        /// </summary>
        public Configuration WrappedConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the resolver to minimal form. It should be null for the initial
        /// configuration, since we don't need to find it's minimal form. This value
        /// should be found by a <see cref="IMinimalFormResolver"/>. For more 
        /// information, see the documentation of the interface.
        /// </summary>
        public IObjectIdResolver ResolverToMinimalForm { get; set; }

        /// <summary>
        /// Gets or sets the previous configuration that was extended to obtain
        /// this one. This configuration will be null for the initial configuration.
        /// </summary>
        public ConfigurationWrapper PreviousConfiguration { get; set; }
    }
}