using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.Objects;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    /// <summary>
    /// Represents a container for <see cref="GeometricalObject"/>s.
    /// </summary>
    internal interface IObjectsContainer
    {
        /// <summary>
        /// Adds a given geometrical object to the container. 
        /// If an equal version of the object is present in the 
        /// container, it will return instance of this internal object. 
        /// Otherwise it will return the object passed object itself.
        /// </summary>
        /// <param name="geometricalObject">The geometrical object.</param>
        /// <param name="originalObject"></param>
        /// <returns>The equal version of the object.</returns>
        ConfigurationObject Add(IAnalyticalObject analyticalObject, ConfigurationObject originalObject);

        /// <summary>
        /// Removes the geometrical object with a given id, if it exists.
        /// </summary>
        /// <param name="id">The id.</param>
        void Remove(int id);

        T Get<T>(ConfigurationObject configurationObject) where T : IAnalyticalObject;

        IAnalyticalObject Get(ConfigurationObject configurationObject);
    }
}