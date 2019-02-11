using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System;

namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// Represents a general constructor of <see cref="ConstructedConfigurationObject"/>s.
    /// </summary>
    public interface IObjectsConstructor
    {
        /// <summary>
        /// Creates a function that can perform the actual geometrical construction of a given
        /// constructed configuration object, finding the needed objects in a given container.
        /// </summary>
        /// <param name="constructedObjects">The object to be constructed.</param>
        /// <returns>The function that can perform the actual construction using the context from a given container.</returns>
        Func<IObjectsContainer, IAnalyticObject> Construct(ConstructedConfigurationObject configurationObject);
    }
}