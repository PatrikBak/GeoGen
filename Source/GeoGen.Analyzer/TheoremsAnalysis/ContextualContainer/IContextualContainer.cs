using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a container that holds <see cref="GeometricalObject"/>s. This container
    /// is responsible for creating them and mapping them to <see cref="AnalyticalObject"/>s.
    /// </summary>
    internal interface IContextualContainer
    {
        /// <summary>
        /// Gets the objects container manager that holds all the  representations of 
        /// the objects inside this container.
        /// </summary>
        IObjectsContainersManager Manager { get; }

        /// <summary>
        /// Gets the geometrical objects matching a given query and casts them
        /// to a given type.
        /// </summary>
        /// <typeparam name="T">The type of objects.</typeparam>
        /// <param name="query">The contextual container query.</param>
        /// <returns>The objects.</returns>
        IEnumerable<T> GetGeometricalObjects<T>(ContexualContainerQuery query) where T : GeometricalObject;

        /// <summary>
        /// Gets the analytical representation of a given geometrical object in a given objects container.
        /// </summary>
        /// /// <typeparam name="T">The wanted type of the analytical object.</typeparam>
        /// <param name="geometricalObject">The geometrical object.</param>
        /// <param name="objectsContainer">The objects container.</param>
        /// <returns>The analytical object.</returns>
        T GetAnalyticalObject<T>(GeometricalObject geometricalObject, IObjectsContainer objectsContainer) where T : AnalyticalObject;
    }
}