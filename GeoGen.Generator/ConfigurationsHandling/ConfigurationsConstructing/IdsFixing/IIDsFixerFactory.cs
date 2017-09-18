using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.IdsFixing
{
    /// <summary>
    /// Represents a factory for creating and caching <see cref="IIdsFixer"/>s
    /// corresponding to <see cref="DictionaryObjectIdResolver"/>s.
    /// </summary>
    internal interface IIdsFixerFactory
    {
        /// <summary>
        /// Creates an ids fixer corresponding to a given
        /// dictionary object id resolver.
        /// </summary>
        /// <param name="resolver">The dictionry object id resolver.</param>
        /// <returns>The ids fixer.</returns>
        IIdsFixer CreateFixer(DictionaryObjectIdResolver resolver);
    }
}