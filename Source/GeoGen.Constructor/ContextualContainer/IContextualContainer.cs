using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a container that holds <see cref="GeometricalObject"/>s. This container
    /// is responsible for creating them and mapping them between <see cref="IAnalyticObject"/>s
    /// with respect to <see cref="IObjectsContainer"/>s.
    /// </summary>
    public interface IContextualContainer
    {
        /// <summary>
        /// Gets the geometrical objects matching a given query and casts them to a given type.
        /// </summary>
        /// <typeparam name="T">The type of objects.</typeparam>
        /// <param name="query">The query that we want to perform.</param>
        /// <returns>The queried objects.</returns>
        IEnumerable<T> GetGeometricalObjects<T>(ContextualContainerQuery query) where T : GeometricalObject;

        /// <summary>
        /// Gets the geometrical objects of the requested type that corresponds to a given configuration object.
        /// </summary>
        /// <typeparam name="T">The type of the geometrical object.</typeparam>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The corresponding geometrical object.</returns>
        T GetGeometricalObject<T>(ConfigurationObject configurationObject) where T : GeometricalObject;

        /// <summary>
        /// Gets the analytic representation of a given geometrical object in a given objects container.
        /// </summary>
        /// /// <typeparam name="T">The wanted type of the analytic object.</typeparam>
        /// <param name="geometricalObject">The geometrical object.</param>
        /// <param name="objectsContainer">The objects container.</param>
        /// <returns>The analytic object represented by the given geometrical object in the given container.</returns>
        T GetAnalyticObject<T>(GeometricalObject geometricalObject, IObjectsContainer objectsContainer) where T : IAnalyticObject;

        /// <summary>
        /// Recreates the underlying analytic objects that this container maps to <see cref="GeometricalObject"/>s.
        /// This method doesn't get delete the <see cref="GeometricalObject"/>s that container already created.
        /// </summary>
        /// <param name="successful">true, if the reconstruction was successful; false otherwise.</param>
        void TryReconstruct(out bool successful);
    }
}