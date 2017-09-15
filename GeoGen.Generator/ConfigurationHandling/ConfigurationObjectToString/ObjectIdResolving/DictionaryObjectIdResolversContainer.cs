using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Core.Utilities.Variations;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving
{
    class DictionaryObjectIdResolversContainer : IDictionaryObjectIdResolversContainer
    {
        private readonly List<DictionaryObjectIdResolver> _resolvers = new List<DictionaryObjectIdResolver>();

        private readonly IVariationsProvider<int> _variationsProvider;

        public DictionaryObjectIdResolversContainer(IVariationsProvider<int> variationsProvider)
        {
            _variationsProvider = variationsProvider;
        }

        public void Initialize(IEnumerable<LooseConfigurationObject> looseConfigurationObjects)
        {
            var ids = looseConfigurationObjects
                    .Select(o => o.Id ?? throw new GeneratorException("Object must have ids"))
                    .ToList();

            var variationsCounter = 0;

            var newResolvers = _variationsProvider
                    .GetVariations(ids, ids.Count)
                    .Select
                    (
                        variation =>
                        {
                            var counter = 0;
                            var dictionary = variation.ToDictionary(i => ids[counter++]);

                            return new DictionaryObjectIdResolver(dictionary, variationsCounter++);
                        }
                    );

            _resolvers.SetItems(newResolvers);
        }

        public IEnumerator<DictionaryObjectIdResolver> GetEnumerator()
        {
            return _resolvers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}