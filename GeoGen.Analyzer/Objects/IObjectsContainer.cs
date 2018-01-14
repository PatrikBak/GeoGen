using System;
using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a container that handles mapping <see cref="ConfigurationObject"/>
    /// to their analytical representations, i.e. <see cref="AnalyticalObject"/>s.
    /// It takes care of resolving duplicate objects. 
    /// </summary>
    internal interface IObjectsContainer
    {
        void Reconstruct();

        /// <summary>
        /// Adds a given object to the container. If the analytical version 
        /// of the object is already present in the container, then it will return
        /// the instance the <see cref="ConfigurationObject"/> that represents the 
        /// given object. If the object is new, it will return the original object.
        /// </summary>
        /// <param name="analyticalObject">The analytical object.</param>
        /// <param name="configurationObject">The configuration object.</param>
        /// <param name="constructor"></param>
        /// <returns>The representation of an equal object.</returns>
        List<ConfigurationObject> Add(IEnumerable<ConfigurationObject> objects, Func<IObjectsContainer, List<AnalyticalObject>> constructor);

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
        T Get<T>(ConfigurationObject configurationObject) where T : AnalyticalObject;

        /// <summary>
        /// Gets the analytical representation of a given configuration object. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The analytical object.</returns>
        AnalyticalObject Get(ConfigurationObject configurationObject);
    }
}