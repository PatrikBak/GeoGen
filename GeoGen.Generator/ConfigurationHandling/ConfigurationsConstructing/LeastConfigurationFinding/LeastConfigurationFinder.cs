using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities.Variations;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConfigurationHandling.ConfigurationToString;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationsConstructing.LeastConfigurationFinding
{
    internal class LeastConfigurationFinder : ILeastConfigurationFinder
    {
        private readonly IVariationsProvider<int> _variationsProvider;

        private readonly IConfigurationToStringProvider _configurationToStringProvider;

        private readonly IConfigurationObjectToStringProviderFactory _objectToStringFactory;

        public LeastConfigurationFinder
        (
            IVariationsProvider<int> variationsProvider,
            IConfigurationToStringProvider configurationToStringProvider,
            IConfigurationObjectToStringProviderFactory objectToStringFactory
        )
        {
            _variationsProvider = variationsProvider ?? throw new ArgumentNullException(nameof(variationsProvider));
            _configurationToStringProvider = configurationToStringProvider ?? throw new ArgumentNullException(nameof(configurationToStringProvider));
            _objectToStringFactory = objectToStringFactory ?? throw new ArgumentNullException(nameof(objectToStringFactory));
        }

        public DictionaryObjectIdResolver FindLeastConfiguration(Configuration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var looseObjectsIds = configuration.LooseObjects
                    .Select(o => o.Id ?? throw new GeneratorException("Imposible"))
                    .ToList();

            Dictionary<int, int> FromVariation(IEnumerable<int> variation)
            {
                var counter = 0;

                return variation.ToDictionary(i => looseObjectsIds[counter++]);
            }

            var resolverString = _variationsProvider
                    .GetVariations(looseObjectsIds, looseObjectsIds.Count)
                    .Select
                    (
                        variation =>
                        {
                            var dictionary = FromVariation(variation);

                            var resolver = new DictionaryObjectIdResolver(dictionary);

                            var customProvider = _objectToStringFactory.CreateCustomProvider(resolver);

                            var stringVersion = _configurationToStringProvider.ConvertToString(configuration, customProvider);

                            return new {Resolver = resolver, StringVersion = stringVersion};
                        }
                    );

            string leastString = null;
            DictionaryObjectIdResolver result = null;

            foreach (var value in resolverString)
            {
                var lessThanLeast = string.Compare(value.StringVersion, leastString, StringComparison.Ordinal) < 0;

                if (leastString == null || lessThanLeast)
                {
                    leastString = value.StringVersion;
                    result = value.Resolver;
                }
            }

            return result;
        }
    }
}