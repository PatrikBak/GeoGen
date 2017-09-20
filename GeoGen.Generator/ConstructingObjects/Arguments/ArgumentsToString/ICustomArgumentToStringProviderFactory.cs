using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;

namespace GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString
{
    /// <summary>
    /// Represents a factory that creates and caches <see cref="CustomArgumentToStringProvider"/>
    /// objects correspondng to <see cref="IObjectToStringProvider"/> objects.
    /// </summary>
    internal interface ICustomArgumentToStringProviderFactory
    {
        /// <summary>
        /// Gets the argument to string provider corresponding to a given
        /// object to string provider.
        /// </summary>
        /// <param name="provider">The object to string provider.</param>
        /// <returns>The argument to string provider.</returns>
        CustomArgumentToStringProvider GetProvider(IObjectToStringProvider provider);
    }
}