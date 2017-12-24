using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a resolver of ids of configuration objects.
    /// The simple implementation would be just to simply return the id 
    /// (<see cref="DefaultObjectIdResolver"/>), but for example
    /// <see cref="LeastConfigurationFinder"/> uses <see cref="DictionaryObjectIdResolver"/>.
    /// These resolvers are supposed to associated with <see cref="IObjectToStringConverter"/>s,
    /// that may cache it's results. Therefore they need to be uniquely identified. 
    /// </summary>
    internal interface IObjectIdResolver
    {
        /// <summary>
        /// Gets the id of the resolver.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Resolves the id of a given configuration object.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The id.</returns>
        int ResolveId(ConfigurationObject configurationObject);
    }
}