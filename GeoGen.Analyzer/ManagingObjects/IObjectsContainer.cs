using System;
using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a container that handles mapping of <see cref="ConfigurationObject"/>
    /// to their analytical representations, i.e. <see cref="AnalyticalObject"/>s.
    /// It's able to reconstruct all objects in the container.
    /// </summary>
    internal interface IObjectsContainer
    {
        /// <summary>
        /// Adds given objects to the container. The analytic versions of these objects
        /// will be constructed using a provided constructor function. This function is
        /// important to give the container an ability to reconstruct itself. This method
        /// returns either null, when the construction can't be performed, or a list
        /// of configuration objects. In this list, every configuration objects
        /// corresponds to the object in the provided objects list. If the 
        /// analytical version of the object is already present in the container, 
        /// then these objects will be the same, otherwise the object in the list will
        /// be the one that representation the duplicate object.
        /// </summary>
        /// <param name="objects">The analytical objects to be constructed.</param>
        /// <param name="constructor">The function that performs the construction.</param>
        /// <returns>null, if the construction failed; or the representation of equal objects from the container.</returns>
        List<ConfigurationObject> Add(IEnumerable<ConfigurationObject> objects, Func<IObjectsContainer, List<AnalyticalObject>> constructor);

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

        /// <summary>
        /// Gets the configuration object that corresponds to a given analytical object.
        /// </summary>
        /// <param name="analyticalObject">The analytical object.</param>
        /// <returns>The configuration objects, if there's an appropriate one; null otherwise.</returns>
        ConfigurationObject Get(AnalyticalObject analyticalObject);

        /// <summary>
        /// Reconstructs all objects in the container. In general, it might happen that
        /// the reconstruction fails (not all objects will be constructible). This method
        /// will try to perform the reconstruction until it's successful. 
        /// </summary>
        void Reconstruct();
    }
}