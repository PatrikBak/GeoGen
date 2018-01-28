using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a service that takes cares actual construction of <see cref="ConstructedConfigurationObject"/>s
    /// into <see cref="AnalyticalObject"/>s. 
    /// </summary>
    public interface IGeometryRegistrar
    {
        /// <summary>
        /// Registers given objects into all objects containers. The objects must be the result of a single
        /// construction.
        /// </summary>
        /// <param name="constructedObjects">The objects to be constructed.</param>
        /// <returns>The result of the registration.</returns>
        RegistrationResult Register(List<ConstructedConfigurationObject> constructedObjects);
    }
}