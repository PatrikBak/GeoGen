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

namespace GeoGen.Generator.NInject
{
    /// <summary>
    /// A NInject module that binds things from the Core module.
    /// </summary>
    public class GeneratorModule : NinjectModule
    {
        /// <summary>
        /// Loads all bindings.
        /// </summary>
        public override void Load()
        {
            Bind<IGeneratorFactory>().To<GeneratorFactory>().InSingletonScope();
            Bind<IObjectsConstructor>().To<ObjectsConstructor>().InSingletonScope();
            Bind<IConstructionsContainer>().To<ConstructionsContainer>().InSingletonScope();
            Bind<IConstructionSignatureMatcher>().To<ConstructionSignatureMatcher>().InSingletonScope();
            Bind<IArgumentsGenerator>().To<ArgumentsGenerator>().InSingletonScope();
            Bind<IArgumentsListContainerFactory>().To<ArgumentsListContainerFactory>().InSingletonScope();
            Bind<IArgumentsListToStringProvider>().To<ArgumentsListToStringProvider>().InSingletonScope();
            Bind<IConfigurationsContainer>().To<ConfigurationsContainer>().InSingletonScope();
            Bind<IConfigurationConstructor>().To<ConfigurationConstructor>().InSingletonScope();
            Bind<ILeastConfigurationFinder>().To<LeastConfigurationFinder>().InSingletonScope();
            Bind<IConfigurationToStringProvider>().To<ConfigurationToStringProvider>().InSingletonScope();
            Bind<ICustomFullObjectToStringProviderFactory>().To<CustomFullObjectToStringProviderFactory>().InSingletonScope();
            Bind<IDictionaryObjectIdResolversContainer>().To<DictionaryObjectIdResolversContainer>().InSingletonScope();
            Bind<IIdsFixerFactory>().To<IdsFixerFactory>().InSingletonScope();
            Bind<IConfigurationObjectsContainer>().To<ConfigurationObjectsContainer>().InSingletonScope();
            Bind<IConfigurationsHandler>().To<ConfigurationsHandler>().InSingletonScope();
            Bind<DefaultObjectIdResolver>().ToSelf().InSingletonScope();
            Bind<DefaultObjectToStringProvider>().ToSelf().InSingletonScope();
            Bind<DefaultFullObjectToStringProvider>().ToSelf().InSingletonScope();
        }
    }
}