using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationToString;
using GeoGen.Generator.ConfigurationsHandling.ObjectsContainer;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsToString;
using GeoGen.Generator.ConstructingObjects.Arguments.Containers;
using Moq;

namespace GeoGen.Generator.Test.TestHelpers
{
    internal class ToStringHelper
    {
        public static string ArgsToString(IReadOnlyList<ConstructionArgument> arg)
        {
            var resolver = new DefaultObjectIdResolver();
            var def = new DefaultObjectToStringProvider(resolver);
            var argument = new CustomArgumentToStringProvider(def, "; ");
            var defaultArgument = new DefaultArgumentToStringProvider(def);

            return new ArgumentsListToStringProvider(defaultArgument, ", ").ConvertToString(arg, argument);
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
            var argsToString = new ArgumentsListToStringProvider(argument);
            var objectToString = new CustomFullObjectToStringProvider(factory, argsToString, resolver);

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
            var argsToString = new ArgumentsListToStringProvider(argument);

            return $"{obj.Construction.Id}{argsToString.ConvertToString(obj.PassedArguments)}[{obj.Index}]";
        }


        public static void AddToContainerRecursive(IArgumentContainer container, List<ConstructionArgument> args)
        {
            void Add(ConstructionArgument arg)
            {
                if (arg is ObjectConstructionArgument)
                {
                    var result = container.AddArgument(arg);
                    arg.Id = result.Id;
                    return;
                }

                var setArg = arg as SetConstructionArgument ?? throw new Exception();
                foreach (var argument in setArg.PassedArguments)
                {
                    Add(argument);
                }

                var result2 = container.AddArgument(setArg);
                setArg.Id = result2.Id;
            }

            foreach (var argument in args)
            {
                Add(argument);
            }
        }
    }
}