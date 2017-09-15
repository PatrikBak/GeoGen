using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationToString;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;

namespace GeoGen.Generator.Test.TestHelpers
{
    internal class ToStringHelper
    {
        public static string ArgsToString(IReadOnlyList<ConstructionArgument> arg)
        {
            var provider = new DefaultObjectToStringProvider();

            return new ArgumentsToStringProvider(provider, ", ", " ").ConvertToString(arg);
        }

        public static string ConfigurationAsString(Configuration configuration, bool full = true)
        {
            var provider = new ConfigurationToStringProvider();
            var def = new DefaultObjectToStringProvider();
            var argsToString = new ArgumentsToStringProvider(def);
            var resolver = new DefaultObjectIdResolver();
            var objectToString = new CustomFullObjectToStringProvider(argsToString, resolver);

            if (full)
                return provider.ConvertToString(configuration, objectToString);

            return provider.ConvertToString(configuration, new DefaultObjectToStringProvider());
        }

        public static string ObjectAsString(ConfigurationObject configurationObject)
        {
            if (configurationObject is LooseConfigurationObject)
                return configurationObject.Id.Value.ToString();

            var obj = configurationObject as ConstructedConfigurationObject;

            var def = new DefaultObjectToStringProvider();
            var argsToString = new ArgumentsToStringProvider(def);

            return $"{obj.Construction.Id}{argsToString.ConvertToString(obj.PassedArguments)}[{obj.Index}]";
        }
    }
}