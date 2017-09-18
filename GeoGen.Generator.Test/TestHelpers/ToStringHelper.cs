using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationToString;
using GeoGen.Generator.Constructing.Arguments.ArgumentsToString;
using Moq;

namespace GeoGen.Generator.Test.TestHelpers
{
    internal class ToStringHelper
    {
        public static string ArgsToString(IReadOnlyList<ConstructionArgument> arg)
        {
            var resolver = new DefaultObjectIdResolver();
            var def = new DefaultObjectToStringProvider(resolver);
            var argument = new CustomArgumentToStringProvider(def);
            var defaultArgument = new DefaultArgumentToStringProvider(def);
            var mock = new Mock<ICustomArgumentToStringProviderFactory>();
            mock.Setup(s => s.GetProvider(It.IsAny<IObjectToStringProvider>()))
                    .Returns<IObjectToStringProvider>(p => new CustomArgumentToStringProvider(p, " "));

            return new ArgumentsListToStringProvider(mock.Object, defaultArgument, ", ").ConvertToString(arg, def);
        }

        private static int _argumentId;

        public static void SetIds(IEnumerable<ConstructionArgument> args)
        {
            void SetId(ConstructionArgument arg)
            {
                if (arg is ObjectConstructionArgument)
                {
                    arg.Id = _argumentId++;
                    return;
                }

                var setArg = (SetConstructionArgument) arg;
                setArg.Id = _argumentId++;

                foreach (var passedArg in setArg.PassedArguments)
                {
                    SetId(passedArg);
                }
            }

            foreach (var arg in args)
            {
                SetId(arg);
            }
        }

        public static string ConfigurationAsString(Configuration configuration, bool full = true)
        {
            var provider = new ConfigurationToStringProvider();
            var resolver = new DefaultObjectIdResolver();
            var def = new DefaultObjectToStringProvider(resolver);
            var argument = new DefaultArgumentToStringProvider(def);
            var factory = new CustomArgumentToStringProviderFactory();
            var argsToString = new ArgumentsListToStringProvider(factory, argument);
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
            var argument = new DefaultArgumentToStringProvider(def);
            var factory = new CustomArgumentToStringProviderFactory();
            var argsToString = new ArgumentsListToStringProvider(factory, argument);

            return $"{obj.Construction.Id}{argsToString.ConvertToString(obj.PassedArguments)}[{obj.Index}]";
        }
    }
}