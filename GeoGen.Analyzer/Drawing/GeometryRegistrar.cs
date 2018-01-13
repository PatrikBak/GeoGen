using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IGeometryRegistrar"/>.
    /// </summary>
    internal sealed class GeometryRegistrar : IGeometryRegistrar
    {
        private readonly IConstructorsResolver _resolver;

        private readonly ITheoremsContainer _theoremsContainer;

        private readonly IObjectsContainersManager _containers;

        private readonly HashSet<int> _resolvedIds;

        public GeometryRegistrar
        (
                IConstructorsResolver resolver,
                ITheoremsContainer theoremsContainer,
                IObjectsContainersManager containers
        )
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _theoremsContainer = theoremsContainer ?? throw new ArgumentNullException(nameof(theoremsContainer));
            _containers = containers ?? throw new ArgumentNullException(nameof(containers));
            _resolvedIds = new HashSet<int>();
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

            List<Theorem> defaultTheorems = null;

            RegistrationResult Register() => RegisterObjects(constructedObjects, out defaultTheorems);

            var result = _containers.ExecuteAndResolvePossibleIncosistencies(Register);
            
            // If result is not ok, we can just directly return it
            if (result != RegistrationResult.Ok)
                return result;

            // Otherwise we need to mark the result as resolved
            foreach (var configurationObject in constructedObjects)
            {
                // Pull id
                var id = configurationObject.Id?? throw new AnalyzerException("Id must be set.");

                // To mark the object's id as resolved
                _resolvedIds.Add(id);
            }

            // And we can register the default theorems
            foreach (var theorem in defaultTheorems)
            {
                _theoremsContainer.Add(theorem);
            }

            // And finally we can return the result (which should be OK)
            return result;
        }

        private RegistrationResult RegisterObjects(List<ConstructedConfigurationObject> objects, out List<Theorem> theorems)
        {
            // Initialize variables
            bool? canBeConstructed = null;
            List<ConfigurationObject> duplicates = null;

            // Perform the construction
            var constructorOutput = _resolver.Resolve(objects[0].Construction).Construct(objects);

            // Pull the constructor function
            var constructorFunction = constructorOutput.ConstructorFunction;

            // Add objects to all containers
            foreach (var container in _containers)
            {
                // Add objects
                var containerResult  = container.Add(objects, constructorFunction);

                // If they are null, that means the construction can't be done 
                // in this container 
                if (containerResult == null)
                {
                    // However if some container marked this objects as constructible,
                    // then we have inconsistency
                    if (canBeConstructed != null && canBeConstructed.Value)
                        throw new InconsistentContainersException(container);

                    // Otherwise we can mark the object as not constructible
                    canBeConstructed = false;

                    // And move to the next container
                    continue;
                }

                // If we got here, the construction is possible
                canBeConstructed = true;

                // Let's find the local duplicates
                var localDuplicates = Enumerable.Range(0, containerResult.Count)
                        .Select(i => containerResult[i] == objects[i] ? null : containerResult[i])
                        .ToList();

                // If duplicates have been already set and they are not equal to the
                // current duplicates, then we have inconsistency
                if (duplicates != null && !duplicates.SequenceEqual(localDuplicates))
                    throw new InconsistentContainersException(container);

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