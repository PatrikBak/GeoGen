using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a service that is able to analyze a configuration and 
    /// find out if it contains that are not needed to be used to define
    /// any of given geometrical objects. This is useful when the provided
    /// geometrical objects should represent a theorem and we want that the
    /// theorem uses all objects from the configuration.
    /// </summary>
    internal interface INeedlessObjectsAnalyzer
    {
        /// <summary>
        /// Finds out if a given configuration contains needless objects that are not
        /// used in the definition of any one a provided geometrical object.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="geometricalObjects">The geometrical objects.</param>
        /// <returns>true, if there is a needless object; false otherwise.</returns>
        bool ContainsNeedlessObjects(Configuration configuration, IEnumerable<GeometricalObject> geometricalObjects);
    }
}