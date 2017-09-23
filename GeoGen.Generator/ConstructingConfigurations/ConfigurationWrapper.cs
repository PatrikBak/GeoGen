using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Generator.ConstructingObjects.Arguments.Container;

namespace GeoGen.Generator.ConstructingConfigurations
{
    /// <summary>
    /// An internal wrapper class for a <see cref="Configuration"/>, containing some
    /// additional data regarding the configuration. 
    /// </summary>
    internal class ConfigurationWrapper
    {
        /// <summary>
        /// Gets or sets the configuration object that this class wraps.
        /// </summary>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// Gets or sets the precalculated configuration objects map. This map is
        /// useful during the objects construction process, since we need to have
        /// objects distinguished by their type.
        /// </summary>
        public ConfigurationObjectsMap ConfigurationObjectsMap { get; set; }

        /// <summary>
        /// Gets or sets the dictionary mapping construction ids to <see cref="IArgumentsListContainer"/>
        /// that holds arguments that are not allowed to be used for the construction when the 
        /// configuration is being extended.
        /// 
        /// TODO: Do we even need this? The containers will contain two types of objects:
        /// 
        ///       1. The objects already present in the configuration
        ///       2. The objects that formally exist, but geometrically either don't exist, or are 
        ///          geometrically the same as some already present in the configuration.
        ///       
        ///       Both of these two types of configurations will be excluded in the analyses process,
        ///       which I believe will be pretty fast. Now we need to fix the container ids, which is not
        ///       that fast. On the other hand, we would have more configurations to be processed with
        ///       the least configuration finder, which is not that fast as well. It certainly needs to 
        ///       be tried out.
        /// </summary>
        public Dictionary<int, IArgumentsListContainer> ForbiddenArguments { get; set; }
    }
}