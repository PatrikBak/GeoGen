using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    internal class ComposedConstructor : IComposedConstructor
    {
        private readonly ComposedConstruction _construction;

        private readonly IConstructorsResolver _resolver;

        private readonly IObjectsContainerFactory _factory;

        public ComposedConstructor(ComposedConstruction construction, IConstructorsResolver resolver, IObjectsContainerFactory factory)
        {
            _construction = construction;
            _resolver = resolver;
            _factory = factory;
        }

        public ConstructorOutput Construct(List<ConstructedConfigurationObject> constructedObjects)
        {
            // Extract the input objects
            var inputObjects = ExtraxtInputObject(constructedObjects[0].PassedArguments);

            // Pull the loose objects (that should correspond to the flatten ones)
            var looseObjects = _construction.ParentalConfiguration.LooseObjects;

            // Construct the function
            List<AnalyticalObject> ConstructorFunction(IObjectsContainer container)
            {
                // Initialize the internal container for sub-results
                var internalContainer = _factory.CreateContainer();

                // Create constructor function
                List<AnalyticalObject> InternalConstructorFunction(IObjectsContainer c)
                {
                    var result = new List<AnalyticalObject>();

                    for (var i = 0; i < looseObjects.Count; i++)
                    {
                        // Take input object
                        var inputObject = inputObjects[i];

                        // Take real object
                        var realObjectAnalytical = container.Get(inputObject);

                        // Add this object to the results list
                        result.Add(realObjectAnalytical);
                    }

                    return result;
                }

                internalContainer.Add(looseObjects, InternalConstructorFunction);

                // Initialize the constructed objects
                foreach (var internalConstructedObjects in _construction.ParentalConfiguration.GroupConstructedObjects())
                {
                    // Find the constructor
                    var constructor = _resolver.Resolve(internalConstructedObjects[0].Construction);

                    // Find the constructor function
                    var constructorFunction = constructor.Construct(internalConstructedObjects).ConstructorFunction;

                    // Add objects to the container
                    var result = internalContainer.Add(internalConstructedObjects, constructorFunction);

                    // Find out if we have correct result
                    var correctResult = result != null && result.SequenceEqual(internalConstructedObjects);

                    // If not, the whole construction is not possible
                    if (!correctResult)
                        return null;
                }

                // Return the result
                return _construction.ConstructionOutput.Select(internalContainer.Get).ToList();
            }

            // Return the result
            return new ConstructorOutput
            {
                ConstructorFunction = ConstructorFunction,
                Theorems = new List<Theorem>()
            };
        }

        /// <summary>
        /// Finds all objects in the arguments and flattens them to the list.
        /// </summary>
        /// <param name="arguments">The arguments list.</param>
        /// <returns>The objects list.</returns>
        private List<ConfigurationObject> ExtraxtInputObject(IReadOnlyList<ConstructionArgument> arguments)
        {
            // Prepare the result
            var result = new List<ConfigurationObject>();

            // Go trough the arguments
            foreach (var argument in arguments)
            {
                // Visit all objects of a given one and add them to the list
                argument.Visit(result.Add);
            }

            // Return the result
            return result;
        }
    }
}