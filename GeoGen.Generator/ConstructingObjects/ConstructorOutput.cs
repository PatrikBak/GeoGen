using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// An internal sealed class representing the output from a constructor.
    /// </summary>
    internal sealed class ConstructorOutput
    {
        /// <summary>
        /// Gets or sets the initial configuration wrapper that was extended.
        /// </summary>
        public ConfigurationWrapper InitialConfiguration { get; set; }

        /// <summary>
        /// Gets or set the list of outputted constructed configuration objects.
        /// </summary>
        public List<ConstructedConfigurationObject> ConstructedObjects { get; set; }
    }
}