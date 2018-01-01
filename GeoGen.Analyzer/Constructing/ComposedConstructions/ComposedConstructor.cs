using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Theorems;

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
            // Flatten the input objects
            var inputObjects = Flatten(constructedObjects[0].PassedArguments);

            // Pull the loose objects (that should correspond to the flatten ones)
            var looseObjects = _construction.ParentalConfiguration.LooseObjects;

            // Construct the function
            List<IAnalyticalObject> ConstructorFunction(IObjectsContainer container)
            {
                // Initialize the internal container for sub-results
                var internalContainer = _factory.CreateContainer();

                // Create constructor function
                List<IAnalyticalObject> InternalConstructorFunction(IObjectsContainer c)
                {
                    var result = new List<IAnalyticalObject>();

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

        private static List<ConfigurationObject> Flatten(IEnumerable<ConstructionArgument> arguments)
        {
            return arguments.SelectMany(Flatten).ToList();
        }

        private static IEnumerable<ConfigurationObject> Flatten(ConstructionArgument argument)
        {
            if (argument is ObjectConstructionArgument objectArgument)
            {
                return new[] {objectArgument.PassedObject};
            }

            var setArgument = (SetConstructionArgument) argument;

            return setArgument.PassedArguments.SelectMany(Flatten).ToList();
        }
    }
}