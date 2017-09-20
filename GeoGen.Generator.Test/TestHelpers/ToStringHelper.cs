using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConstructingConfigurations.ConfigurationToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;

namespace GeoGen.Generator.Test.TestHelpers
{
    internal class ToStringHelper
    {
        public static string ArgsToString(IReadOnlyList<ConstructionArgument> arg)
        {
            var defaultResolver = new DefaultObjectIdResolver();
            var defaultProvider = new DefaultObjectToStringProvider(defaultResolver);

            return new ArgumentsListToStringProvider(defaultProvider).ConvertToString(arg);
        }

        public static string ConfigurationAsString(Configuration configuration)
        {
            var configurationProvider = new ConfigurationToStringProvider();
            var defaultResolver = new DefaultObjectIdResolver();
            var defaultProvider = new DefaultObjectToStringProvider(defaultResolver);
            var argsProvider = new ArgumentsListToStringProvider(defaultProvider);
            var fullProvider = new CustomFullObjectToStringProvider(argsProvider, defaultResolver);

            return configurationProvider.ConvertToString(configuration, fullProvider);
        }
    }
}