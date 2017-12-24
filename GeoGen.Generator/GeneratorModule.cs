using GeoGen.Core.Generator;
using GeoGen.Core.NInject;

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
            BindDefiningGeneratorScope<IGenerator, Generator>("maximalNumberOfIterations", input => input.MaximalNumberOfIterations);
            BindFactoryInSingletonScope<IGeneratorFactory>();

            // Constructing objects
            BindInGeneratorScope<IArgumentsGenerator, ArgumentsGenerator>();
            BindFactoryInGeneratorScope<IArgumentsListContainerFactory>();
            BindInTransietScope<IArgumentsListContainer, ArgumentsListContainer>();
            BindInGeneratorScope<IDefaultArgumentsListToStringConverter, DefaultArgumentsListToStringConverter>();
            BindInGeneratorScope<IDefaultObjectToStringConverter, DefaultObjectToStringConverter>();
            BindInGeneratorScope<IDefaultObjectIdResolver, DefaultObjectIdResolver>();
            BindInGeneratorScope<IConstructionSignatureMatcher, ConstructionSignatureMatcher>();
            BindInGeneratorScope<IConstructionsContainer, ConstructionsContainer>("initialConstructions", input => input.Constructions);
            BindInGeneratorScope<IObjectsConstructor, ObjectsConstructor>();

            // Handling configurations
            BindInGeneratorScope<IConfigurationsManager, ConfigurationsManager>("initialConfiguration", input => input.InitialConfiguration);
            BindInGeneratorScope<IConfigurationObjectsContainer, ConfigurationObjectsContainer>("initialConfiguration", input => input.InitialConfiguration);
            BindInGeneratorScope<IConfigurationsHandler, ConfigurationsHandler>();
            BindInGeneratorScope<IConfigurationConstructor, ConfigurationConstructor>();
            BindInGeneratorScope<IIdsFixerFactory, IdsFixerFactory>();
            BindInGeneratorScope<ILeastConfigurationFinder, LeastConfigurationFinder>();
            BindInGeneratorScope<ICustomFullObjectToStringProviderFactory, CustomFullObjectToStringProviderFactory>();
            BindInGeneratorScope<IDictionaryObjectIdResolversContainer, DictionaryObjectIdResolversContainer>();
            BindInGeneratorScope<IConfigurationToStringProvider, ConfigurationToStringProvider>("initialConfiguration", input => input.InitialConfiguration);
            BindInGeneratorScope<IArgumentsListToStringConverter, ArgumentsListToStringConverter>();
            BindInGeneratorScope<IDefaultFullObjectToStringConverter, DefaultFullObjectToStringConverter>();
            BindInGeneratorScope<IConfigurationsContainer, ConfigurationsContainer>();
            BindInGeneratorScope<IDefaultConfigurationToStringConverter, DefaultConfigurationToStringConverter>();
            BindInGeneratorScope<IResolversComposer, ResolversComposer>();
        }
    }
}