using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Constructing;
using GeoGen.Analyzer.Drawing;
using GeoGen.Analyzer.Objects.GeometricalObjects.Container;
using GeoGen.Analyzer.Theorems;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Utilities;

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

        private readonly HashSet<int> _resolvedIds;

        public GeometryRegistrar
        (
                Configuration initialConfiguration,
                IConstructorsResolver resolver,
                ITheoremsContainer theoremsContainer,
                IContextualContainer contextualContainer,
                IObjectsContainersManager containers
        )
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _theoremsContainer = theoremsContainer ?? throw new ArgumentNullException(nameof(theoremsContainer));
            _contextualContainer = contextualContainer ?? throw new ArgumentNullException(nameof(contextualContainer));
            _containers = containers ?? throw new ArgumentNullException(nameof(containers));
            _resolvedIds = new HashSet<int>();
            Initialize(initialConfiguration);
        }

        private void Initialize(Configuration configuration)
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
                return RegistrationResult.Ok;

            // Let the helper method construct the result. Before returning it, 
            // we need to analyze it
            var result = RegisterObjects(constructedObjects, out var defaultTheorems);

            // If result is not ok, we can just directly return it
            if (result != RegistrationResult.Ok)
                return result;

            // Otherwise we need to mark the result as resolved
            foreach (var configurationObject in constructedObjects)
            {
                // Pull id
                var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set.");

                // To mark the object's id as resolved
                _resolvedIds.Add(id);
            }

            // And we can register the default theorems
            foreach (var theorem in defaultTheorems)
            {
                _theoremsContainer.Add(theorem);
            }

            // We further need to register the objects to the contextual container
            foreach (var configurationObject in constructedObjects)
            {
                _contextualContainer.Add(configurationObject);
            }

            // And finally we can return the result (which should be OK)
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

                // If they are null, that means the construction can't be done 
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

                // If duplicates have been already set and they are not equal to the
                // current duplicates, then we have inconsistency
                if (duplicates != null && !duplicates.SequenceEqual(localDuplicates))
                    throw new AnalyzerException("Inconsistent containers.");

                // Otherwise we can mark the current duplicates
                duplicates = localDuplicates;
            }

            // Set the theorems
            theorems = constructorOutput.Theorems;

            // If we can't construct it, we return unconstructible result
            if (!canBeConstructed ?? throw new AnalyzerException("Impossible"))
                return RegistrationResult.Unconstructible;

            // Let's find out if there are any duplicates
            var anyDuplicate = duplicates.Any(o => o != null);

            // According to that we'll return the final result
            return anyDuplicate ? RegistrationResult.Duplicates : RegistrationResult.Ok;
        }

        private bool ObjectsAreAlreadyPresent(IReadOnlyCollection<ConstructedConfigurationObject> objects)
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