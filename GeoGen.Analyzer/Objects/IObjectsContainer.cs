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
        /// <returns>The equal version of the object.</returns>
        GeometricalObject Add(GeometricalObject geometricalObject);

        /// <summary>
        /// Removes the geometrical object with a given id, if it exists.
        /// </summary>
        /// <param name="id">The id.</param>
        void Remove(int id);

        GeometricalObject this[int id] { get; }
    }
}