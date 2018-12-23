using GeoGen.Core;

namespace GeoGen.Generator
{
    public interface ILooseObjectsIdResolver
    {
        /// <summary>
        /// Resolves the id of a given loose object.
        /// </summary>
        /// <param name="looseObject">The loose object.</param>
        /// <returns>The resolved id.</returns>
        int ResolveId(LooseConfigurationObject looseObject);
    }
}
