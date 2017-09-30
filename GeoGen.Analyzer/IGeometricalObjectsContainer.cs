using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a container for geometrical objects. It's supposed to 
    /// handle adding new objects, find out if the object can be constructed
    /// and find duplicate objects.
    /// </summary>
    internal interface IGeometricalObjectsContainer
    {
        /// <summary>
        /// Initializes the container with a given configuration.
        /// </summary>
        /// <param name="looseObjects">The loose objects enumerable.</param>
        void Initialize(Configuration looseObjects);

        /// <summary>
        /// Adds a given object to the container.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <param name="duplicateVersion">The out parameter where the reference to duplicate object will be set.</param>
        /// <returns>true, if the container's content has changed, false otherwise.</returns>
        bool Add(ConfigurationObject configurationObject, out ConfigurationObject duplicateVersion);

        /// <summary>
        /// Removes given configuration objects from the container, if they exist.
        /// </summary>
        /// <param name="objects">The configuration objects enumerable.</param>
        void Remove(IEnumerable<ConfigurationObject> objects);
    }
}