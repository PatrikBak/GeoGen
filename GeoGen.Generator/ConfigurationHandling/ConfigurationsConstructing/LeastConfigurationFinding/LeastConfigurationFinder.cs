using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IConfigurationToStringProvider _configurationToStringProvider;

        private readonly ICustomFullObjectToStringProviderFactory _customFullObjectToStringFactory;

        private readonly IDictionaryObjectIdResolversContainer _dictionaryObjectIdResolversContainer;

        public LeastConfigurationFinder
        (
            IConfigurationToStringProvider configurationToStringProvider,
            ICustomFullObjectToStringProviderFactory customFullObjectToStringFactory,
            IDictionaryObjectIdResolversContainer dictionaryObjectIdResolversContainer
        )
        {
            _configurationToStringProvider = configurationToStringProvider ?? throw new ArgumentNullException(nameof(configurationToStringProvider));
            _customFullObjectToStringFactory = customFullObjectToStringFactory ?? throw new ArgumentNullException(nameof(customFullObjectToStringFactory));
            _dictionaryObjectIdResolversContainer = dictionaryObjectIdResolversContainer ?? throw new ArgumentNullException(nameof(dictionaryObjectIdResolversContainer));
        }

        public static Stopwatch s_toString = new Stopwatch();
        public static Stopwatch s_iterating = new Stopwatch();

        public DictionaryObjectIdResolver FindLeastConfiguration(Configuration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            string leastString = null;
            DictionaryObjectIdResolver result = null;

            foreach (var resolver in _dictionaryObjectIdResolversContainer)
            {
                var customProvider = _customFullObjectToStringFactory.GetCustomProvider(resolver);

                s_toString.Start();
                var stringVersion = _configurationToStringProvider.ConvertToString(configuration, customProvider);
                s_toString.Stop();

                var lessThanLeast = string.Compare(stringVersion, leastString, StringComparison.Ordinal) < 0;

                if (leastString == null || lessThanLeast)
                {
                    leastString = stringVersion;
                    result = resolver;
                }
            }

            return result;
        }
    }
}