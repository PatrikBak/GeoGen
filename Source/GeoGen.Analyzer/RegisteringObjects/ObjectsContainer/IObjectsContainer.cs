using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a container that handles mapping of <see cref="ConfigurationObject"/> to their analytic 
    /// representations, i.e. <see cref="IAnalyticObject"/>s. This container assumes each object is present
    /// exactly ones and doesn't allow duplicity. It is also able to reconstruct all its objects.
    /// </summary>
    public interface IObjectsContainer : IEnumerable<(ConfigurationObject configurationObject, IAnalyticObject analyticObject)>
    {
        /// <summary>
        /// Adds given configuration objects to the container. Their analytic versions are constructed
        /// using a provided constructor function. This method should be used only when we're sure that
        /// the construction will succeed (for example, for constructing loose objects).
        /// </summary>
        /// <param name="objects">The configuration objects to be added to the container.</param>
        /// <param name="constructor">The constructor function that performs the construction of the analytic versions of the objects.</param>
        void Add(IEnumerable<ConfigurationObject> objects, Func<List<IAnalyticObject>> constructor);

        /// <summary>
        /// Tries to add a given configuration object to the container. The analytic version of the object is 
        /// constructed using the provided constructor function. The method sets the out parameters indicating
        /// whether the construction was successful and whether there is a duplicate version of the object.
        /// The object is added if and only if the construction is successful and there is no duplicity.
        /// </summary>
        /// <param name="configurationObject">The configuration object to be added to the container.</param>
        /// <param name="constructor">The constructor function that performs the construction of the analytic version of the object.</param>
        /// <param name="equalObject">If there already is the same object in the container, this value will be set to that object. Otherwise it will be null.</param>
        /// <param name="objectConstructed">Indicates if the construction was successful.</param>
        void TryAdd(ConfigurationObject configurationObject, Func<IAnalyticObject> constructor, out bool objectConstructed, out ConfigurationObject equalObject);

        /// <summary>
        /// Gets the analytic representation of a given configuration object. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The analytic object of a given configuration object.</returns>
        IAnalyticObject Get(ConfigurationObject configurationObject);

        /// <summary>
        /// Gets the configuration object corresponding to a given analytic object.
        /// </summary>
        /// <param name="analyticObject">The analytic object.</param>
        /// <returns>The configuration object of a given analytic object.</returns>
        ConfigurationObject Get(IAnalyticObject analyticObject);

        /// <summary>
        /// Finds out if a given analytic object is present if the container.
        /// </summary>
        /// <param name="analyticObject">The analytic object.</param>
        /// <returns>true, if the object is present in the container; false otherwise.</returns>
        bool Contains(IAnalyticObject analyticObject);

        /// <summary>
        /// Finds out if a given configuration object is present if the container.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>true, if the object is present in the container; false otherwise.</returns>
        bool Contains(ConfigurationObject configurationObject);

        /// <summary>
        /// Tries to reconstruct all the objects in the container. If the reconstruction fails, 
        /// the content of the container will remain unchanged.
        /// </summary>
        /// <param name="reconstructionSuccessful">true, if the reconstruction was successful; false otherwise.</param>
        void TryReconstruct(out bool reconstructionSuccessful);
    }
}