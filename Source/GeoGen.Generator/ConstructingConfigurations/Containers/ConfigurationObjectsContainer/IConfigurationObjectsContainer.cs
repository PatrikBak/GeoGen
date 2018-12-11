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
        /// Adds a given configuration object to the container. The object must not be identified 
        /// while it's being added. If an equal version of the object is present in the container, 
        /// the object won't be added and the <paramref name="equalObject"/> will be set to that
        /// equal object. Otherwise the <paramref name="equalObject"/> will be set to null.
        /// </summary>
        /// <param name="configurationObject">The configuration object to be added.</param>
        /// <param name="equalObject">Either the equal version of the passed object from the container (if there's any), or null.</param>
        void Add(ConfigurationObject configurationObject, out ConfigurationObject equalObject);
    }
}