using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.ConstructingConfigurations.IdsFixing
{
    /// <summary>
    /// Represents a factory for creating <see cref="IIdsFixer"/> 
    /// implementations that use a given <see cref="DictionaryObjectIdResolver"/>.
    /// </summary>
    internal interface IIdsFixerFactory
    {
        /// <summary>
        /// Creates an ids fixer corresponding to a given
        /// dictionary object id resolver.
        /// </summary>
        /// <param name="resolver">The dictionary object id resolver.</param>
        /// <returns>The ids fixer.</returns>
        IIdsFixer CreateFixer(DictionaryObjectIdResolver resolver);
    }
}