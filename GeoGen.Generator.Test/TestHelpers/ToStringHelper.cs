using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConstructingConfigurations.ConfigurationToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
using Moq;

namespace GeoGen.Generator.Test.TestHelpers
{
    internal class ToStringHelper
    {
        public static string ArgsToString(IReadOnlyList<ConstructionArgument> arg)
        {
            var resolver = new DefaultObjectIdResolver();
            var def = new DefaultObjectToStringProvider(resolver);

            return new ArgumentsListToStringProvider(def).ConvertToString(arg);
        }

        public static string ConfigurationAsString(Configuration configuration, bool full = true)
        {
            var provider = new ConfigurationToStringProvider();
            var resolver = new DefaultObjectIdResolver();
            var def = new DefaultObjectToStringProvider(resolver);
            var argsToString = new ArgumentsListToStringProvider(def);
            var objectToString = new CustomFullObjectToStringProvider(argsToString, resolver);

            if (full)
                return provider.ConvertToString(configuration, objectToString);

            return provider.ConvertToString(configuration, def);
        }

        public static string ObjectAsString(ConfigurationObject configurationObject)
        {
            if (configurationObject is LooseConfigurationObject)
                return configurationObject.Id.Value.ToString();

            var obj = configurationObject as ConstructedConfigurationObject;

            var resolver = new DefaultObjectIdResolver();
            var def = new DefaultObjectToStringProvider(resolver);
            var argsToString = new ArgumentsListToStringProvider(def);

            return $"{obj.Construction.Id}{argsToString.ConvertToString(obj.PassedArguments)}[{obj.Index}]";
        }


        
    }
}