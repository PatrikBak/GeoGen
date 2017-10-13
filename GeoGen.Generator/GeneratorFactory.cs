using System;
using System.Linq;
using GeoGen.Generator.ConfigurationsHandling;
using GeoGen.Generator.ConstructingConfigurations;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;
using GeoGen.Generator.ConstructingObjects;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IGeneratorFactory"/>.
    /// </summary>
    internal sealed class GeneratorFactory : IGeneratorFactory
    {
        #region Private fields

        /// <summary>
        /// The constructions container.
        /// </summary>
        private readonly IConstructionsContainer _constructionsContainer;

        /// <summary>
        /// The objects constructor.
        /// </summary>
        private readonly IObjectsConstructor _objectsConstructor;

        /// <summary>
        /// The configurations handler.
        /// </summary>
        private readonly IConfigurationsHandler _configurationsHandler;

        /// <summary>
        /// The configurations container.
        /// </summary>
        private readonly IConfigurationsContainer _configurationsContainer;

        /// <summary>
        /// The dictionary object id resolvers container
        /// </summary>
        private readonly IDictionaryObjectIdResolversContainer _dictionaryIdResolversContainer;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new generator factory with all needed dependencies.
        /// </summary>
        /// <param name="constructionsContainer">The constructions container.</param>
        /// <param name="objectsConstructor">The objects constructor.</param>
        /// <param name="configurationsHandler">The configurations handler.</param>
        /// <param name="configurationsContainer">The configurations container.</param>
        /// <param name="dictionaryIdResolversContainer">The dictionary id resolvers container.</param>
        public GeneratorFactory
        (
            IConstructionsContainer constructionsContainer,
            IObjectsConstructor objectsConstructor,
            IConfigurationsHandler configurationsHandler,
            IConfigurationsContainer configurationsContainer,
            IDictionaryObjectIdResolversContainer dictionaryIdResolversContainer
        )
        {
            _constructionsContainer = constructionsContainer ?? throw new ArgumentNullException(nameof(configurationsContainer));
            _objectsConstructor = objectsConstructor ?? throw new ArgumentNullException(nameof(objectsConstructor));
            _configurationsHandler = configurationsHandler ?? throw new ArgumentNullException(nameof(configurationsHandler));
            _configurationsContainer = configurationsContainer ?? throw new ArgumentNullException(nameof(configurationsContainer));
            _dictionaryIdResolversContainer = dictionaryIdResolversContainer ?? throw new ArgumentNullException(nameof(dictionaryIdResolversContainer));
        }

        #endregion

        #region IGeneratorFactory implementation

        /// <summary>
        /// Creates a generator for a given generator input.
        /// </summary>
        /// <param name="generatorInput">The generator input.</param>
        /// <returns>The generator.</returns>
        public IGenerator CreateGenerator(GeneratorInput generatorInput)
        {
            _constructionsContainer.Initialize(generatorInput.Constructions);
            _configurationsContainer.Initialize(generatorInput.InitialConfiguration);
            _dictionaryIdResolversContainer.Initialize(generatorInput.InitialConfiguration.LooseObjects.ToList());
            var iterations = generatorInput.MaximalNumberOfIterations;

            return new Generator(_configurationsContainer, _objectsConstructor, _configurationsHandler, iterations);
        }

        #endregion
    }
}