using System;
using System.Diagnostics;
using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationToString;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.LeastConfigurationFinding
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
        public static bool finding;

        public DictionaryObjectIdResolver FindLeastConfiguration(Configuration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            string leastString = null;
            DictionaryObjectIdResolver result = null;

            foreach (var resolver in _dictionaryObjectIdResolversContainer)
            {
                s_iterating.Start();
                var customProvider = _customFullObjectToStringFactory.GetCustomProvider(resolver);
                s_iterating.Stop();

                finding = true;
                s_toString.Start();
                var stringVersion = _configurationToStringProvider.ConvertToString(configuration, customProvider);
                s_toString.Stop();
                finding = false;

                s_iterating.Start();
                var lessThanLeast = string.Compare(stringVersion, leastString, StringComparison.Ordinal) < 0;

                if (leastString == null || lessThanLeast)
                {
                    leastString = stringVersion;
                    result = resolver;
                }
                s_iterating.Stop();
            }

            return result;
        }
    }
}