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

        private readonly IObjectsContainersHolder _containers;

        private readonly HashSet<int> _resolvedIds;

        public GeometryRegistrar
        (
            IConstructorsResolver resolver,
            ITheoremsContainer theoremsContainer,
            IContextualContainer contextualContainer,
            IObjectsContainersHolder containers)
        {
            _resolver = resolver;
            _theoremsContainer = theoremsContainer;
            _contextualContainer = contextualContainer;
            _containers = containers;
            _resolvedIds = new HashSet<int>();
        }

        public void Initialize(Configuration configuration)
        {
            _containers.Initialize(configuration.LooseObjects);

            // TODO: Fix this
            // I don't think this will work with if there are at least two objects constructed
            // with the same construction and arguments (i.e. that differs only by indices)
            // This can be fixed by finding these objects in the constructed objects list.
            // It shouldn't be hard because they are supposed to be there consecutively
            foreach (var constructedObject in configuration.ConstructedObjects)
            {
                var objectAsList = new List<ConstructedConfigurationObject> {constructedObject};

                var registrationResult = Register(objectAsList);

                if (!registrationResult.CanBeConstructed)
                    throw new AnalyzerException("Unconstructible situation.");

                if (!registrationResult.DuplicateObjects.Empty())
                    throw new AnalyzerException("Situation with duplicate objects");
            }
        }

        public RegistrationResult Register(List<ConstructedConfigurationObject> objects)
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            if (objects.Empty())
                throw new ArgumentException("Objects can't be empty");

            // First we check if the objects haven't been registered before.
            if (ObjectsAreAlreadyPresent(objects))
            {
                // If yes, then the state of the theorems and object containers won't change
                return new RegistrationResult
                {
                    CanBeConstructed = true,
                    DuplicateObjects = new Dictionary<ConfigurationObject, ConfigurationObject>()
                };
            }

            // Let the helper method construct the result. Before returning it, 
            // we need to analyze it
            var result = RegisterObjects(objects, out List<Theorem> defaultTheorems);

            // If we can construct all the objects without duplications
            if (result.CanBeConstructed && result.DuplicateObjects.Empty())
            {
                // Then we can register the default theorems
                foreach (var theorem in defaultTheorems)
                {
                    _theoremsContainer.Add(theorem);
                }

                // Loop over objects
                foreach (var configurationObject in objects)
                {
                    // To mark the object's id as resolved
                    _resolvedIds.Add(configurationObject.Id ?? throw new AnalyzerException("Id must be set."));

                    // And add the object to the contextual container
                    _contextualContainer.Add(configurationObject);
                }
            }
            // Otherwise we want to return the state of the holder and its components
            // to the state before calling this method
            else
            {
                // The only change that might cause inconsistency was adding objects
                // to the containers. Therefore we iterate over all objects
                foreach (var configurationObject in objects)
                {
                    // And over all containers
                    foreach (var container in _containers)
                    {
                        // Remove the object from the container
                        container.Remove(configurationObject);
                    }
                }
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
                // in this container and so we don't need to continue
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

                // Otherwise the object is fine
                canBeConstructed = true;

                // Initializes duplicates list
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

                    // If the container's result is not our object, i.e. the object
                    // isn't already present in the container, we're fine
                    if (containerResult != originalObject)
                        continue;

                    // Update the duplicates list
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
                DuplicateObjects = duplicateObjects,
                CanBeConstructed = canBeConstructed ?? throw new AnalyzerException("Impossible")
            };
        }

        private bool ObjectsAreAlreadyPresent(List<ConstructedConfigurationObject> objects)
        {
            var ids = objects.Select(obj => obj.Id ?? throw new AnalyzerException("Id must be set"));

            var presentIds = ids.Count(id => _resolvedIds.Contains(id));

            if (presentIds == objects.Count)
                return true;

            if (presentIds == 0)
                return false;

            throw new AnalyzerException("Some objects from the same construction are present and other are not.");
        }
    }
}