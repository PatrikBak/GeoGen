using GeoGen.Core;

namespace GeoGen.Generator
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
            // General code
            BindDefiningGeneratorScope<IGenerator, Generator>("initialConfiguration", input => input.InitialConfiguration);
            BindFactoryInSingletonScope<IGeneratorFactory>();

            // Constructing objects
            BindInGeneratorScope<IArgumentsGenerator, ArgumentsGenerator>();
            BindFactoryInGeneratorScope<IArgumentsContainerFactory>();
            BindInTransietScope<IArgumentsContainer, ArgumentsContainer>();
            BindInGeneratorScope<IDefaultArgumentsToStringConverter, DefaultArgumentsToStringConverter>();
            BindInGeneratorScope<IDefaultObjectToStringConverter, DefaultObjectToStringConverter>();
            BindInGeneratorScope<IDefaultObjectIdResolver, DefaultObjectIdResolver>();
            BindInGeneratorScope<IConstructionSignatureMatcher, ConstructionSignatureMatcher>();
            BindInGeneratorScope<IConstructionsContainer, ConstructionsContainer>("constructions", input => input.Constructions);
            BindInGeneratorScope<IObjectsConstructor, ObjectsConstructor>();

            // Handling configurations
            BindInGeneratorScope<IConfigurationsManager, ConfigurationsManager>("initialConfiguration", input => input.InitialConfiguration);
            BindInGeneratorScope<IConfigurationObjectsContainer, ConfigurationObjectsContainer>("initialConfiguration", input => input.InitialConfiguration);
            BindInGeneratorScope<IConfigurationConstructor, ConfigurationConstructor>();
            BindInGeneratorScope<IMinimalFormResolver, MinimalFormResolver>();
            BindInGeneratorScope<IObjectIdResolversContainer, ObjectIdResolversContainer>("looseObjects", input => input.InitialConfiguration.LooseObjectsHolder);
            BindInGeneratorScope<IConfigurationToStringProvider, ConfigurationToStringProvider>();
            BindInGeneratorScope<IArgumentsToStringProvider, ArgumentsToStringProvider>();
            BindInGeneratorScope<IDefaultFullObjectToStringConverter, DefaultFullObjectToStringConverter>();
            BindInGeneratorScope<IConfigurationsContainer, ConfigurationsContainer>();
            BindInGeneratorScope<IConfigurationsValidator, ConfigurationsResolver>();
            BindInGeneratorScope<IFullConfigurationToStringConverter, FullConfigurationToStringConverter>();
            BindInGeneratorScope<IFullObjectToStringConvertersFactory, FullObjectToStringConvertersFactory>();
            BindFactoryInGeneratorScope<IAutocacheFullObjectToStringConverterFactory>();
            BindInTransietScope<IAutocacheFullObjectToStringConverter, AutocacheFullObjectToStringConverter>();
        }
    }
}