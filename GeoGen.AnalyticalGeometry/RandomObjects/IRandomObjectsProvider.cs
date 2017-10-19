using GeoGen.AnalyticalGeometry.Objects;

namespace GeoGen.AnalyticalGeometry.RandomObjects
{
    /// <summary>
    /// Represents a simple generator of mutually distinct analytical objects.
    /// </summary>
    public interface IRandomObjectsProvider
    {
        /// <summary>
        /// Generates the next random object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>The next random object.</returns>
        IAnalyticalObject NextRandomObject<T>() where T : IAnalyticalObject;
    }
}