using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// An internal wrapper class for a <see cref="Core.Configurations.Configuration"/>, containing some
    /// additional data regarding the configuration. 
    /// </summary>
    internal sealed class ConfigurationWrapper
    {
        /// <summary>
        /// TODO
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Gets or sets the configuration object that this sealed class wraps.
        /// </summary>
        public Configuration Configuration { get; set; }

        public ConfigurationWrapper PreviousConfiguration { get; set; }

        public IObjectIdResolver ResolverToMinimalForm { get; set; }

        /// <summary>
        /// Gets or sets the list of configuration objects that are not last added.
        /// All objects are union of original objects and last added objects.
        /// </summary>
        public IReadOnlyList<ConfigurationObject> OriginalObjects { get; set; }

        /// <summary>
        /// Gets or sets the list of constructed configuration objects 
        /// that are last added objects to this configuration.
        /// All objects are union of original objects and last added objects.
        /// </summary>
        public IReadOnlyList<ConstructedConfigurationObject> LastAddedObjects { get; set; }
    }
}