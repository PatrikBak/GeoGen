using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a general constructor of <see cref="ConstructedConfigurationObject"/>s.
    /// </summary>
    public interface IObjectsConstructor
    {
        /// <summary>
        /// Creates a function that can perform the actual geometric construction of a given
        /// constructed configuration object, finding the needed objects in a given picture.
        /// </summary>
        /// <param name="constructedObjects">The object to be constructed.</param>
        /// <returns>The function that can perform the actual construction using the context from a given picture.</returns>
        Func<IPicture, IAnalyticObject> Construct(ConstructedConfigurationObject configurationObject);
    }
}