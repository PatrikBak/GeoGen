using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;

namespace GeoGen.Generator.ConfigurationsHandling.ObjectsContainer
{
    /// <summary>
    /// Represents a container for <see cref="ConfigurationObject"/>. It implements
    /// the <see cref="IEnumerable{T}"/> interface, whete T is <see cref="ConfigurationObject"/>.
    /// It takes care of assigning ids to configuration object - in fact, it is not
    /// supposed to happen elsewhere. 
    /// </summary>
    internal interface IConfigurationObjectsContainer : IEnumerable<ConfigurationObject>
    {
        /// <summary>
        /// Gets the default complex configuration object to string provider that is used by the container.
        /// </summary>
        DefaultFullObjectToStringProvider ConfigurationObjectToStringProvider { get; }

        /// <summary>
        /// Adds a given object to a container. The current id of the
        /// object will be gnored.If an equal version of the object 
        /// is present in the container, it will return instance of 
        /// this internal object. Otherwise it will return this 
        /// object with set id.
        /// </summary>
        /// <param name="constructedConfigurationObject">The configuration object.</param>
        /// <returns>The identified version of the configuration object.</returns>
        ConstructedConfigurationObject Add(ConstructedConfigurationObject constructedConfigurationObject);

        /// <summary>
        /// Initializes the container with a given configuratin.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        void Initialize(Configuration configuration);

        /// <summary>
        /// Gets the object with a given id. Throws an exception, if not present.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The configuration object with the given id.</returns>
        ConfigurationObject this[int id] { get; }
    }
}