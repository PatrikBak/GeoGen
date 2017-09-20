using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConstructingObjects.Arguments.Containers;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationsContainer
{
    /// <summary>
    /// An internal wrapper class for a <see cref="Configuration"/>, containing some
    /// additional information regarding the configuration. During the generation 
    /// process, these instances are preferred.
    /// </summary>
    internal class ConfigurationWrapper
    {
        /// <summary>
        /// Gets or sets the configuration object that this class wraps.
        /// </summary>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the pre-calculated configuration objects map.
        /// </summary>
        public ConfigurationObjectsMap ConfigurationObjectsMap { get; set; }

        /// <summary>
        /// Gets or sets the dictionary map mapping construction ids to <see cref="IArgumentsListContainer"/>
        /// that holds arguments that are not allowed to be used for the construction when the 
        /// configuration is being extended.
        /// </summary>
        public Dictionary<int, IArgumentsListContainer> ForbiddenArguments { get; set; }

        public Dictionary<IObjectToStringProvider, SortedDictionary<string, ConstructedConfigurationObject>> Sorted { get; set; }
    }
}