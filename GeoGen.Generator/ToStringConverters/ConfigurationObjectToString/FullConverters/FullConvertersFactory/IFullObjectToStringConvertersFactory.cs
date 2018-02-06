namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a factory for getting an <see cref="IFullObjectToStringConverter"/>
    /// for a given <see cref="IObjectIdResolver"/>.
    /// </summary>
    internal interface IFullObjectToStringConvertersFactory
    {
        /// <summary>
        /// Gets a full object to string converted associated with a given
        /// object id resolver.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <returns>The converter.</returns>
        IFullObjectToStringConverter Get(IObjectIdResolver resolver);
    }
}