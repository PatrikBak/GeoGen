using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IObjectsConstructor"/>.
    /// It uses a given <see cref="IConstructionsContainer"/> that contains
    /// all constructions to be performed on given configurations. The 
    /// arguments for new <see cref="ConstructedConfigurationObject"/>s
    /// are generated using a provided <see cref="IArgumentsGenerator"/>.
    /// </summary>
    internal class ObjectsConstructor : IObjectsConstructor
    {
        #region Private fields

        /// <summary>
        /// The container of all constructions that are performed on given configurations.
        /// </summary>
        private readonly IConstructionsContainer _constructionsContainer;

        /// <summary>
        /// The generator of arguments that are passed to newly created constructed objects.
        /// </summary>
        private readonly IArgumentsGenerator _argumentsGenerator;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new objects constructor that takes constructions
        /// from a given constructions container and uses a given arguments
        /// generator to generate arguments that are used to create 
        /// constructed configuration objects.
        /// </summary>
        /// <param name="constructionsContainer">The constructions container.</param>
        /// <param name="argumentsGenerator">The arguments generator.</param>
        public ObjectsConstructor(IConstructionsContainer constructionsContainer, IArgumentsGenerator argumentsGenerator)
        {
            _constructionsContainer = constructionsContainer ?? throw new ArgumentNullException(nameof(constructionsContainer));
            _argumentsGenerator = argumentsGenerator ?? throw new ArgumentNullException(nameof(argumentsGenerator));
        }

        #endregion

        #region IObjectsConstructor implementation

        /// <summary>
        /// Performs all possible constructions on a given configuration.
        /// </summary>
        /// <param name="configurationWrapper">The configuration wrapper.</param>
        /// <returns>The enumerable of constructor outputs.</returns>
        public IEnumerable<ConstructorOutput> GenerateOutput(ConfigurationWrapper configurationWrapper)
        {
            // For each construction create many outputs
            return _constructionsContainer.SelectMany(constructionWrapper =>
            {
                // Generate arguments
                var generatedArguments = _argumentsGenerator.GenerateArguments(configurationWrapper, constructionWrapper);

                // Unwrap construction
                var unwrapedConstruction = constructionWrapper.WrappedConstruction;

                // Cast each arguments list to a new constructor output
                var newOutput = generatedArguments.Select(arguments =>
                {
                    // Construct the needed number of objects
                    var constructedObjects = Enumerable.Range(0, unwrapedConstruction.OutputTypes.Count)
                            .Select(i => new ConstructedConfigurationObject(unwrapedConstruction, arguments, i))
                            .ToList();

                    // Create and return the output
                    return new ConstructorOutput
                    {
                        OriginalConfiguration = configurationWrapper,
                        ConstructedObjects = constructedObjects
                    };
                });

                // Return the enumerable of outputs
                return newOutput;
            });
        }

        #endregion
    }
}