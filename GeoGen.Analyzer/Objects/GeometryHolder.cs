using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Constructing;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Objects
{
    /// <summary>
    /// A default implementation of <see cref="IGeometryHolder"/>. This
    /// class is not thread-safe.
    /// </summary>
    internal class GeometryHolder : IGeometryHolder
    {
        private const int NumberOfContainers = 5;

        private readonly IObjectsConstructor _constructor;

        private readonly IObjectsContainersFactory _factory;

        private readonly ITheoremsContainer _theorems;

        private readonly HashSet<int> _resolvedIds;

        private readonly List<IObjectsContainer> _containers;

        public void Initialize(Configuration configuration)
        {
            var containers = Enumerable.Range(0, NumberOfContainers)
                    .Select(i => _factory.CreateContainer(configuration.LooseObjects));

            _containers.SetItems(containers);

            // TODO: Fix this
            // I don't this will work with if there are at least two objects constructed
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

            // Otherwise we need to construct the objects in all containers along with
            // default theorems. First prepare the list for theorems
            var defaultTheorems = new List<Theorem>();

            // Let the helper method construct the result. Before returning it, 
            // we need to analyze it
            var result = RegisterObjects(objects, defaultTheorems);

            // If we can construct all the objects without duplications
            if (result.CanBeConstructed && result.DuplicateObjects.Empty())
            {
                // Then we can register the default theorems
                foreach (var theorem in defaultTheorems)
                {
                    _theorems.Add(theorem);
                }

                // And mark all object ids as resolved
                foreach (var configurationObject in objects)
                {
                    var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set.");
                    _resolvedIds.Add(id);
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
                        // To make sure the objects will be deleted from all containers
                        var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set.");
                        container.Remove(id);
                    }
                }
            }

            // And finally we can return the result.
            return result;
        }

        private RegistrationResult RegisterObjects(List<ConstructedConfigurationObject> objects, List<Theorem> theorems)
        {
            // First we prepare variables
            var canBeConstructed = true;
            var duplicateObjects = new Dictionary<ConfigurationObject, ConfigurationObject>();
            List<Theorem> defaultTheorems = null;

            // We iterate over all containers
            foreach (var container in _containers)
            {
                // Let the constructor construct the object for new objects
                var constructorOutput = _constructor.Construct(objects, container);

                // Pull constructed geometrical objects
                var constructedObjects = constructorOutput.Objects;

                // If they are null, that means the construction can't be provided 
                // in this container and so we don't need to continue
                if (constructedObjects == null)
                {
                    canBeConstructed = false;
                    break;
                }

                // Initialize a local variable to indicate if we have
                // duplicate objects
                var areThereDuplicates = false;

                // Otherwise we have constructible objects. We need to add them to 
                // the container and check if we have duplicate objects on go
                foreach (var constructedObject in constructedObjects)
                {
                    // Let the container resolve the new object
                    var containerResult = container.Add(constructedObject);

                    // If it returned the same object, then we have a new object
                    // and we can move further
                    if (containerResult == constructedObject)
                        continue;

                    // Otherwise we have duplicate objects. We can pull 
                    // the underlying configurations objects
                    var originalObject = constructedObject.ConfigurationObject;
                    var duplicateObject = containerResult.ConfigurationObject;

                    // Update the duplicates dictionary
                    duplicateObjects.Add(originalObject, duplicateObject);

                    // And set the local variable
                    areThereDuplicates = true;
                }

                // If there are duplicate objects, we don't want to continue
                // with other containers
                if (areThereDuplicates)
                    break;

                // Otherwise we can set the default theorems to the ones gotten from the constructor
                // This might be set uselessly a few times, but it's not a big deal.
                defaultTheorems = constructorOutput.Theorems;
            }

            // After finding all the objects we can set up the provided theorems list
            if (defaultTheorems != null)
                theorems.SetItems(defaultTheorems);

            // And finally construct and return the result
            return new RegistrationResult
            {
                DuplicateObjects = duplicateObjects,
                CanBeConstructed = canBeConstructed
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

        public IEnumerator<IObjectsContainer> GetEnumerator()
        {
            return _containers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}