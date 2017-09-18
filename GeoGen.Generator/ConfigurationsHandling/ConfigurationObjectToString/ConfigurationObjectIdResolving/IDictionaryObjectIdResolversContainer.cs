using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving
{
    /// <summary>
    /// Represents a container of <see cref="DictionaryObjectIdResolver"/> objects. They
    /// are created from an enumerable of <see cref="LooseConfigurationObject"/>, using
    /// all possible permutations of their ids. This is used during the process of
    /// handling symetric configurations.
    /// </summary>
    internal interface IDictionaryObjectIdResolversContainer : IEnumerable<DictionaryObjectIdResolver>
    {
        /// <summary>
        /// Initializes the container with a non-empty list of loose
        /// configuration objects.
        /// </summary>
        /// <param name="looseConfigurationObjects">The loose configuration objects list.</param>
        void Initialize(IReadOnlyList<LooseConfigurationObject> looseConfigurationObjects);
    }
}