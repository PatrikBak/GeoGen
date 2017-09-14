using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConfigurationHandling.ConfigurationToString;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using GeoGen.Generator.Test.ConfigurationHandling.ConfigurationObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.Test.TestHelpers
{
    internal class ToStringHelper
    {
        public static string ArgsToString(IReadOnlyList<ConstructionArgument> arg)
        {
            var provider = new DefaultConfigurationObjectToStringProvider();

            return new ArgumentsToStringProvider(provider, ", ", " ").ConvertToString(arg);
        }

        public static string ConfigurationAsString(Configuration configuration, bool full = true)
        {
            var provider = new ConfigurationToStringProvider();
            var def = new DefaultConfigurationObjectToStringProvider();
            var argsToString = new ArgumentsToStringProvider(def);
            var resolver = new DefaultObjectIdResolver();
            var objectToString = new CustomComplexConfigurationObjectToStringProvider(argsToString, resolver);

            if (full)
                return provider.ConvertToString(configuration, objectToString);

            return provider.ConvertToString(configuration, new DefaultConfigurationObjectToStringProvider());
        }

        public static string ObjectAsString(ConfigurationObject configurationObject)
        {
            if (configurationObject is LooseConfigurationObject)
                return configurationObject.Id.Value.ToString();

            var obj = configurationObject as ConstructedConfigurationObject;

            var def = new DefaultConfigurationObjectToStringProvider();
            var argsToString = new ArgumentsToStringProvider(def);

            return $"{obj.Construction.Id}{argsToString.ConvertToString(obj.PassedArguments)}[{obj.Index}]";
        }
    }
}