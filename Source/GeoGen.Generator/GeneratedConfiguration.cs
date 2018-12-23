using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// An internal wrapper class for a <see cref="Configuration"/>, 
    /// containing additional data regarding the configuration that are used
    /// during the generation process.
    /// </summary>
    public class GeneratedConfiguration : Configuration
    {
        /// <summary>
        /// Gets or sets the previous configuration that was extended to obtain
        /// this one. This configuration will be null for the initial configuration.
        /// </summary>
        public GeneratedConfiguration PreviousConfiguration { get; }

        public GeneratedConfiguration(GeneratedConfiguration currentConfiguration, List<ConstructedConfigurationObject> newObjects)
            : base(currentConfiguration.LooseObjectsHolder, currentConfiguration.ConstructedObjects.Concat(newObjects).ToList())
        {
        }

        public GeneratedConfiguration(Configuration configuration)
            : base(configuration.LooseObjectsHolder, configuration.ConstructedObjects)
        {
        }
    }
}