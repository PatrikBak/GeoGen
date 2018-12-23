using GeoGen.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IObjectsGenerator"/>. It uses a given 
    /// <see cref="IConstructionsContainer"/> that contains all constructions to be 
    /// performed on given configurations. The arguments for new 
    /// <see cref="ConstructedConfigurationObject"/>s are generated using 
    /// an injected <see cref="IArgumentsGenerator"/>.
    /// </summary>
    public class ObjectsGenerator : IObjectsGenerator
    {
        #region Dependencies

        /// <summary>
        /// The container of all constructions that are performed on given configurations.
        /// </summary>
        private readonly IConstructionsContainer _constructionsContainer;

        /// <summary>
        /// The generator of arguments that are passed to newly created constructed objects.
        /// </summary>
        private readonly IArgumentsGenerator _argumentsGenerator;

        /// <summary>
        /// The container of all the generated objects.
        /// </summary>
        private readonly IContainer<ConfigurationObject> _container;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new objects constructor that takes constructions
        /// from a given constructions container and uses a given arguments
        /// generator to generate arguments that are used to create 
        /// constructed configuration objects.
        /// </summary>
        /// <param name="container">The container of all the generated objects.</param>
        /// <param name="constructionsContainer">The constructions container.</param>
        /// <param name="argumentsGenerator">The arguments generator.</param>
        public ObjectsGenerator(IConstructionsContainer constructionsContainer, IContainer<ConfigurationObject> container, IArgumentsGenerator argumentsGenerator)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _constructionsContainer = constructionsContainer ?? throw new ArgumentNullException(nameof(constructionsContainer));
            _argumentsGenerator = argumentsGenerator ?? throw new ArgumentNullException(nameof(argumentsGenerator));
        }

        #endregion

        #region IObjectsConstructor implementation

        /// <summary>
        /// Constructs all possible construction outputs that can be added
        /// constructed from the objects of a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration which objects should be used.</param>
        /// <returns>The enumerable of all possible construction outputs.</returns>
        public IEnumerable<List<ConstructedConfigurationObject>> ConstructPossibleObjects(Configuration configuration)
        {
            // For each construction create many outputs
            return _constructionsContainer.SelectMany(constructionWrapper =>
            {
                // Generate arguments
                var generatedArguments = _argumentsGenerator.GenerateArguments(configuration, constructionWrapper);

                // Unwrap construction
                var unwrapedConstruction = constructionWrapper.WrappedConstruction;

                // Cast each arguments to a new constructor output
                return generatedArguments.Select(arguments => Enumerable.Range(0, unwrapedConstruction.OutputTypes.Count)
                            .Select(i => new ConstructedConfigurationObject(unwrapedConstruction, arguments, i))
                            .ToList());
            })
            // Register them
            .Select(newObjects =>
            {
                // We need to make sure the new objects are registered correctly
                // These objects are not identified yet and we need to be sure
                // there are for further algorithms...
                // Go through all the constructed objects
                for (var objectIndex = 0; objectIndex < newObjects.Count; objectIndex++)
                {
                    // Add the current one
                    _container.Add(newObjects[objectIndex], out var equalObject);

                    // If there is an equal object...
                    if (equalObject != null)
                    {
                        // Reset the position in the output
                        // This step makes sure that equal objects won't be used with
                        // more than one instance. The objects that we will 'rewrite' 
                        // shouldn't be used elsewhere and thus will be eventually 
                        // eaten by the garbage collector
                        newObjects[objectIndex] = (ConstructedConfigurationObject) equalObject;
                    }
                }

                // Return the new objects
                return newObjects;
            });
        }
    
        #endregion
    }
}