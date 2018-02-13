using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a container for <see cref="ConfigurationObject"/>. It's 
    /// supposed to take care of having each object exactly once. It implements
    /// the <see cref="IEnumerable{T}"/> interface, where T is <see cref="ConfigurationObject"/>.
    /// </summary>
    internal interface IConfigurationObjectsContainer : IEnumerable<ConfigurationObject>
    {
        /// <summary>
        /// Adds a given constructed configuration object to the container. 
        /// The current id of the object will be ignored. If an equal version 
        /// of the object is present in the container, it will return the instance of 
        /// this internal object. Otherwise it will return this object with set id.
        /// </summary>
        /// <param name="constructedObject">The constructed configuration object.</param>
        /// <returns>The equal identified version of the constructed configuration object.</returns>
        ConstructedConfigurationObject Add(ConstructedConfigurationObject constructedObject);
    }
}