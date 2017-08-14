using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities.Combinator;
using GeoGen.Core.Utilities.Variations;
using GeoGen.Generator.Wrappers;

namespace GeoGen.Generator.Constructor.Arguments
{
    internal class SignaturesCombinator : ISignaturesCombinator
    {
        private readonly IVariationsProvider<ConfigurationObject> _variationsProvider;

        private readonly ICombinator<ConfigurationObjectType, IEnumerable<ConfigurationObject>> _combinator;

        public IEnumerable<IReadOnlyDictionary<ConfigurationObjectType, IEnumerable<ConfigurationObject>>> Combine(
            ConfigurationWrapper configuration, ConstructionWrapper construction)
        {
            var dictionaryForCombinator = configuration.ObjectTypeToObjects.ToDictionary
            (
                keyValue => keyValue.Key,
                keyValue => _variationsProvider.GetVariations(keyValue.Value, construction.ObjectTypesToNeededCount[keyValue.Key])
            );

            foreach (var result in _combinator.Combine(dictionaryForCombinator))
            {
                yield return result;
            }
        }
    }
}