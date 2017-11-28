using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Constructing;
using GeoGen.Analyzer.Objects.GeometricalObjects.Container;
using GeoGen.Analyzer.Theorems;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Objects
{
    /// <summary>
    /// A default implementation of <see cref="IGeometryRegistrar"/>.
    /// </summary>
    internal sealed class GeometryRegistrar : IGeometryRegistrar
    {
        private readonly IConstructorsResolver _resolver;

        private readonly ITheoremsContainer _theoremsContainer;

        private readonly IContextualContainer _contextualContainer;

        private readonly IObjectsContainersManager _containers;

        private readonly Dictionary<int, RegistrationResult> _resolvedIds;

        public GeometryRegistrar
        (
            IConstructorsResolver resolver,
            ITheoremsContainer theoremsContainer,
            IContextualContainer contextualContainer,
            IObjectsContainersManager containers)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _theoremsContainer = theoremsContainer ?? throw new ArgumentNullException(nameof(theoremsContainer));
            _contextualContainer = contextualContainer ?? throw new ArgumentNullException(nameof(contextualContainer));
            _containers = containers ?? throw new ArgumentNullException(nameof(containers));
            _resolvedIds = new Dictionary<int, RegistrationResult>();
        }

        public void Initialize(Configuration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Pull loose objects
            var looseObjects = configuration.LooseObjects;

            // Initialize the containers
            _containers.Initialize(looseObjects);

            // Initialize the contextual container
            foreach (var configurationObject in looseObjects)
            {
                _contextualContainer.Add(configurationObject);
            }

            if (configuration.ConstructedObjects.Empty())
                return;

            throw new NotImplementedException("Implement initialization of constructed objects");
        }

        public RegistrationResult Register(List<ConstructedConfigurationObject> constructedObjects)
        {
            if (constructedObjects == null)
                throw new ArgumentNullException(nameof(constructedObjects));

            if (constructedObjects.Empty())
                throw new ArgumentException("Objects can't be empty");

            // First we check if the objects haven't been registered before.
            if (ObjectsAreAlreadyPresent(constructedObjects))
            {
                // If yes, then we pull some id
                var id = constructedObjects[0].Id ?? throw new AnalyzerException("Id must be set");

                // And grab the result from the cache
                return _resolvedIds[id];
            }

            // Let the helper method construct the result. Before returning it, 
            // we need to analyze it
            var result = RegisterObjects(constructedObjects, out List<Theorem> defaultTheorems);

            // If we can't construct the objects or there are duplicates
            // we might just result the result directly
            if (!result.CanBeConstructed || !result.GeometricalDuplicates.Empty())
                return result;

            // Cache the result
            foreach (var configurationObject in constructedObjects)
            {
                // Pull id
                var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set.");

                // To mark the object's id as resolved
                _resolvedIds.Add(id, result);
            }

            // Then we can register the default theorems
            foreach (var theorem in defaultTheorems)
            {
                _theoremsContainer.Add(theorem);
            }

            // We further need to register the objects to the contextual container
            foreach (var configurationObject in constructedObjects)
            {
                // And add the object to the contextual container
                _contextualContainer.Add(configurationObject);
            }

            // And finally we can return the result.
            return result;
        }

        private RegistrationResult RegisterObjects(List<ConstructedConfigurationObject> objects, out List<Theorem> theorems)
        {
            // Initialize variables
            bool? canBeConstructed = null;
            List<ConfigurationObject> duplicates = null;

            // Pull the constructor
            var constructor = _resolver.Resolve(objects[0].Construction);

            // Perform the construction
            var constructorOutput = constructor.Construct(objects);

            // We iterate over all containers
            foreach (var container in _containers)
            {
                // Construct the objects
                var constructedObjects = constructorOutput.ConstructorFunction(container);

                // If they are null, that means the construction can't be provided 
                // in this container 
                if (constructedObjects == null)
                {
                    // However if some container marked this objects as constructible,
                    // then we have inconsistency
                    if (canBeConstructed != null && canBeConstructed.Value)
                        throw new AnalyzerException("Inconsistent containers");

                    // Otherwise we can mark the object as not constructible
                    canBeConstructed = false;

                    // And move to the next container
                    continue;
                }

                // If we got here, the construction is possible
                canBeConstructed = true;

                // Now we need to find duplicates that we stored in this list
                // initialized with nulls
                var localDuplicates = Enumerable.Repeat((ConfigurationObject) null, objects.Count).ToList();

                // We iterate over the objects to find duplicates
                for (var i = 0; i < constructedObjects.Count; i++)
                {
                    // Pull the constructed object
                    var constructedObject = constructedObjects[i];

                    // Pull the original object
                    var originalObject = objects[i];

                    // Let the container resolve the new object
                    var containerResult = container.Add(constructedObject, originalObject);

                    // If the container's result is the same as our object, i.e. the object
                    // isn't already present in the container, then the constructed
                    // object is new
                    if (containerResult == originalObject)
                        continue;

                    // Otherwise we update the duplicates list
                    localDuplicates[i] = containerResult;
                }

                if (duplicates != null && !duplicates.SequenceEqual(localDuplicates))
                    throw new AnalyzerException("Inconsistent containers.");

                duplicates = localDuplicates;
            }

            // Set the theorems
            theorems = constructorOutput.Theorems;

            // Initialize duplicates objects dictionary
            var duplicateObjects = new Dictionary<ConfigurationObject, ConfigurationObject>();

            // Construct it by iterating over objects
            for (var i = 0; i < objects.Count; i++)
            {
                var duplicate = duplicates?[i];

                if (duplicate != null)
                    duplicateObjects.Add(objects[i], duplicate);
            }

            // And finally construct and return the result
            return new RegistrationResult
            {
                GeometricalDuplicates = duplicateObjects,
                CanBeConstructed = canBeConstructed ?? throw new AnalyzerException("Impossible")
            };
        }

        private bool ObjectsAreAlreadyPresent(IReadOnlyCollection<ConstructedConfigurationObject> objects)
        {
            var ids = objects.Select(obj => obj.Id ?? throw new AnalyzerException("Id must be set"));

            var presentIds = ids.Count(id => _resolvedIds.ContainsKey(id));

            if (presentIds == objects.Count)
                return true;

            if (presentIds == 0)
                return false;

            throw new AnalyzerException("Some objects from the same construction are present and other are not.");
        }
    }
}