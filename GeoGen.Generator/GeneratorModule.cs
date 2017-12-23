using System;
using System.Linq;
using GeoGen.Core.Generator;
using GeoGen.Core.NInject;
using GeoGen.Generator.ConfigurationsHandling;
using GeoGen.Generator.ConstructingConfigurations;
using GeoGen.Generator.ConstructingConfigurations.ConfigurationToString;
using GeoGen.Generator.ConstructingConfigurations.IdsFixing;
using GeoGen.Generator.ConstructingConfigurations.LeastConfigurationFinding;
using GeoGen.Generator.ConstructingConfigurations.ObjectsContainer;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConstructingObjects;
using GeoGen.Generator.ConstructingObjects.Arguments;
using GeoGen.Generator.ConstructingObjects.Arguments.ArgumentsListToString;
using GeoGen.Generator.ConstructingObjects.Arguments.Container;
using GeoGen.Generator.ConstructingObjects.Arguments.SignatureMatching;
using Ninject.Modules;
using Ninject.Planning.Targets;

namespace GeoGen.Generator.NInject
{
    /// <summary>
    /// A NInject module that binds things from the Core module.
    /// </summary>
    public class GeneratorModule : BaseModule
    {
        /// <summary>
        /// Loads all bindings.
        /// </summary>
        public override void Load()
        {
            BindFactoryInSingletonScope<IGeneratorFactory>();
            BindFactoryInGeneratorScope<IArgumentsListContainerFactory>();

            BindInGeneratorScope<IConstructionSignatureMatcher, ConstructionSignatureMatcher>();
            BindInGeneratorScope<IArgumentsGenerator, ArgumentsGenerator>();
            BindInGeneratorScope<IArgumentsListToStringProvider, ArgumentsListToStringProvider>();
            BindInGeneratorScope<IObjectsConstructor, ObjectsConstructor>();
            BindInGeneratorScope<IConfigurationConstructor, ConfigurationConstructor>();
            BindInGeneratorScope<ILeastConfigurationFinder, LeastConfigurationFinder>();
            BindInGeneratorScope<IConfigurationToStringProvider, ConfigurationToStringProvider>();
            BindInGeneratorScope<ICustomFullObjectToStringProviderFactory, CustomFullObjectToStringProviderFactory>();
            BindInGeneratorScope<IDictionaryObjectIdResolversContainer, DictionaryObjectIdResolversContainer>();
            BindInGeneratorScope<IIdsFixerFactory, IdsFixerFactory>();
            BindInGeneratorScope<IConfigurationsHandler, ConfigurationsHandler>();
            BindInTransietScope<IArgumentsListContainer, ArgumentsListContainer>();

            BindInGeneratorScopeToSelf<DefaultObjectIdResolver>();
            BindInGeneratorScopeToSelf<DefaultObjectToStringProvider>();
            BindInGeneratorScopeToSelf<DefaultFullObjectToStringProvider>();

            BindDefiningGeneratorScope<IGenerator, Generator>("maximalNumberOfIterations", input => input.MaximalNumberOfIterations);
            BindInGeneratorScope<IConstructionsContainer, ConstructionsContainer>("initialConstructions", input => input.Constructions);
            BindInGeneratorScope<IConfigurationObjectsContainer, ConfigurationObjectsContainer>("initialConfiguration", input => input.InitialConfiguration);
            BindInGeneratorScope<IConfigurationsManager, ConfigurationsManager>("initialConfiguration", input => input.InitialConfiguration);
        }
    }
}