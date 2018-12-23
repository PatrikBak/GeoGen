namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// Represents a simple generator of mutually distinct analytic objects.
    /// </summary>
    public interface IRandomObjectsProvider
    {
        /// <summary>
        /// Generates the next random object mutually distinct from 
        /// all previously generated ones.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>The next random object.</returns>
        AnalyticObject NextRandomObject<T>() where T : AnalyticObject;
    }
}