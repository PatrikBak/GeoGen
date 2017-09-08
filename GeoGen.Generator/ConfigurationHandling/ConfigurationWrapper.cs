using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Generator.Constructing.Arguments.Container;

namespace GeoGen.Generator.ConfigurationHandling
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
        /// Gets or sets the pre-calculated dictionary mapping object types to actual
        /// objects of this types contained in the configuration.
        /// </summary>
        public IReadOnlyDictionary<ConfigurationObjectType, List<ConfigurationObject>> ObjectTypeToObjects { get; set; }

        /// <summary>
        /// Gets or sets the dictionary mapping construction ids to <see cref="IArgumentsContainer"/>
        /// that holds arguments that are not allowed to be used for the construction when the 
        /// configuration is being extended.
        /// </summary>
        public Dictionary<int, IArgumentsContainer> ConstructionIdToForbiddenArguments { get; set; }
    }
}