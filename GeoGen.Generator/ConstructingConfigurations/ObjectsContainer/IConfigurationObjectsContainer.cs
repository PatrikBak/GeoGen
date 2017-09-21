using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;

namespace GeoGen.Generator.ConstructingConfigurations.ObjectsContainer
{
    /// <summary>
    /// Represents a container for <see cref="ConfigurationObject"/>. It implements
    /// the <see cref="IEnumerable{T}"/> interface, where T is <see cref="ConfigurationObject"/>.
    /// It's should be   
    /// </summary>
    internal interface IConfigurationObjectsContainer : IEnumerable<ConfigurationObject>
    {
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