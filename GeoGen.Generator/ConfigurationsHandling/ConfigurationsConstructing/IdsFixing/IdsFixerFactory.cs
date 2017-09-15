using System;
using System.Collections.Generic;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConfigurationsHandling.ObjectsContainer;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.IdsFixing
{
    class IdsFixerFactory : IIdsFixerFactory
    {
        private readonly IConfigurationObjectsContainer _configurationObjectsContainer;

        private readonly Dictionary<int, IIdsFixer> _cache;

        public IdsFixerFactory(IConfigurationObjectsContainer configurationObjectsContainer)
        {
            _configurationObjectsContainer = configurationObjectsContainer ?? throw new ArgumentNullException(nameof(configurationObjectsContainer));
            _cache = new Dictionary<int, IIdsFixer>();
        }

        public IIdsFixer CreateFixer(DictionaryObjectIdResolver resolver)
        {
            if (_cache.ContainsKey(resolver.Id))
                return _cache[resolver.Id];

            var newFixer = new IdsFixer(_configurationObjectsContainer, resolver);
            _cache.Add(resolver.Id, newFixer);

            return newFixer;
        }
    }
}
