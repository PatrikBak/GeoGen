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
        /// Gets or sets the configuration object that this sealed class wraps.
        /// </summary>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        public int? Id { get; set; }

        public ConfigurationWrapper PreviousConfiguration { get; set; }

        public IObjectIdResolver ResolverToMinimalForm { get; set; }

        /// <summary>
        /// Gets or sets the precalculated configuration objects map. This map is
        /// useful during the objects construction process, since we need to have
        /// objects distinguished by their type.
        /// </summary>
        public ConfigurationObjectsMap AllObjectsMap { get; set; }

        /// <summary>
        /// Gets or sets the list of configuration objects that are not last added.
        /// All objects are union of original objects and last added objects.
        /// </summary>
        public List<ConfigurationObject> OriginalObjects { get; set; }

        /// <summary>
        /// Gets or sets the list of constructed configuration objects 
        /// that are last added objects to this configuration.
        /// All objects are union of original objects and last added objects.
        /// </summary>
        public List<ConstructedConfigurationObject> LastAddedObjects { get; set; }

        /// <summary>
        /// Gets or sets if this configuration has been excluded from further 
        /// generation process. This happens when it's been found out that the 
        /// configuration contains duplicates or non-constructible points.
        /// </summary>
        public bool Excluded { get; set; }
    }
}