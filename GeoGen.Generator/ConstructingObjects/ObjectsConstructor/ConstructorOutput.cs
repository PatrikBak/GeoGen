using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents an output from a <see cref="IObjectsConstructor"/>.
    /// </summary>
    internal class ConstructorOutput
    {
        /// <summary>
        /// Gets or sets the initial configuration wrapper that was extended.
        /// </summary>
        public ConfigurationWrapper OriginalConfiguration { get; set; }

        /// <summary>
        /// Gets or set the list of outputted constructed configuration objects.
        /// </summary>
        public List<ConstructedConfigurationObject> ConstructedObjects { get; set; }
    }
}