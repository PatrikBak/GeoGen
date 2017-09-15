using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConfigurationHandling.ObjectsContainer;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.IdsFixing
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
