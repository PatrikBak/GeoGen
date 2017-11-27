using GeoGen.AnalyticalGeometry;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    /// <summary>
    /// Represents a container that handles mapping <see cref="ConfigurationObject"/>
    /// to their analytical representations, i.e. <see cref="IAnalyticalObject"/>s.
    /// It takes care of resolving duplicate objects. 
    /// </summary>
    internal interface IObjectsContainer
    {
        /// <summary>
        /// Adds a given object to the container. If the analytical version 
        /// of the object is already present in the container, then it will return
        /// the instance the <see cref="ConfigurationObject"/> that represents the 
        /// given object. If the object is new, it will return the original object.
        /// </summary>
        /// <param name="analyticalObject">The analytical object.</param>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The representation of an equal object.</returns>
        ConfigurationObject Add(IAnalyticalObject analyticalObject, ConfigurationObject configurationObject);

        /// <summary>
        /// Removes a given configuration object from the container. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        void Remove(ConfigurationObject configurationObject);

        /// <summary>
        /// Gets the analytical representation of a given configuration object. 
        /// </summary>
        /// <typeparam name="T">The type of analytical object.</typeparam>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The analytical object.</returns>
        T Get<T>(ConfigurationObject configurationObject) where T : IAnalyticalObject;

        /// <summary>
        /// Gets the analytical representation of a given configuration object. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The analytical object.</returns>
        IAnalyticalObject Get(ConfigurationObject configurationObject);
    }
}