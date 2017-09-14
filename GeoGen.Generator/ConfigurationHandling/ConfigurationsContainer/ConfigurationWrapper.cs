using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Generator.Constructing.Arguments.Container;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationsContainer
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
        /// Gets or sets the dictionary map mapping construction ids to <see cref="IArgumentsContainer"/>
        /// that holds arguments that are not allowed to be used for the construction when the 
        /// configuration is being extended.
        /// </summary>
        public Dictionary<int, IArgumentsContainer> ForbiddenArguments { get; set; }
    }
}