using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

                // Initialize the loose objects
                for (var i = 0; i < looseObjects.Count; i++)
                {
                    // Take loose object
                    var looseObject = looseObjects[i];

                    // Take real object
                    var realObjectAnalytical = container.Get(inputObjects[i]);

                    // Register them to the container
                    internalContainer.Add(realObjectAnalytical, looseObject);
                }

                // Initialize the constructed objects
                foreach (var internalConstructedObjects in _construction.ParentalConfiguration.GroupConstructedObjects())
                {
                    // Find the constructor
                    var constructor = _resolver.Resolve(internalConstructedObjects[0].Construction);

                    // Perform the construction
                    var objects = constructor.Construct(internalConstructedObjects).ConstructorFunction(internalContainer);

                    // If these objects are not constructible, then 
                    // even the whole construction is not possible for this input
                    if (objects == null)
                    {
                        return null;
                    }

                    // And add the new objects to the container
                    for (var i = 0; i < objects.Count; i++)
                    {
                        internalContainer.Add(objects[i], internalConstructedObjects[i]);
                    }
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