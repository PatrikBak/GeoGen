using System.Collections.Generic;
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
        /// Gets or sets the id of the wrapper. The sole purpose of the id is 
        /// to have a fast way to look up the previous configuration in the dictionary.
        /// It shouldn't be used to decide whether two configurations are formally equal (or symmetric).
        /// However, all created wrappers should have non-null id.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Gets or sets the actual configuration that this class wraps.
        /// </summary>
        public Configuration WrappedConfiguration { get; set; }

        /// <summary>
        /// Gets or sets the list of constructed configuration objects 
        /// that are the last added objects to this configuration.
        /// </summary>
        public IReadOnlyList<ConstructedConfigurationObject> LastAddedObjects { get; set; }

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