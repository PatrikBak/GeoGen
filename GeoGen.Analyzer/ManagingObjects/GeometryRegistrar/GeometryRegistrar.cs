using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IGeometryRegistrar"/>.
    /// </summary>
    internal class GeometryRegistrar : IGeometryRegistrar
    {
        #region Private fields

        /// <summary>
        /// The resolver for getting constructors.
        /// </summary>
        private readonly IConstructorsResolver _resolver;

        /// <summary>
        /// The container for keeping default theorems.
        /// </summary>
        private readonly ITheoremsContainer _theoremsContainer;

        /// <summary>
        /// The manager of all containers.
        /// </summary>
        private readonly IObjectsContainersManager _containers;

        /// <summary>
        /// The dictionary mapping ids of resolved constructed objects to their 
        /// registration results.
        /// </summary>
        private readonly Dictionary<int, RegistrationResult> _cachedResults;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="resolver">The resolver to find the right constructions for constructions.</param>
        /// <param name="container">The container for keeping default theorems.</param>
        /// <param name="manager">The manager of all objects container where we register the objects.</param>
        public GeometryRegistrar(IConstructorsResolver resolver, ITheoremsContainer container, IObjectsContainersManager manager)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _theoremsContainer = container ?? throw new ArgumentNullException(nameof(container));
            _containers = manager ?? throw new ArgumentNullException(nameof(manager));
            _cachedResults = new Dictionary<int, RegistrationResult>();
        }

        #endregion

        #region IGeometryRegistrar implementation

        /// <summary>
        /// Registers given objects into all objects containers. The objects must be the result of a single
        /// construction.
        /// </summary>
        /// <param name="constructedObjects">The objects to be constructed.</param>
        /// <returns>The result of the registration.</returns>
        public RegistrationResult Register(List<ConstructedConfigurationObject> constructedObjects)
        {
            // Check if objects haven't been already registered 
            var cachedResult = FindCachedRegistrationResult(constructedObjects);

            // If it's not null, we can directly return it
            if (cachedResult != null)
                return cachedResult.Value;

            // Otherwise we need to register them. First prepare
            // a list holding default theorems
            List<Theorem> defaultTheorems = null;

            // Prepare function that performs the actual registration
            RegistrationResult RegistrationFunction() => RegisterObjects(constructedObjects, out defaultTheorems);

            // Execute the registration (with auto-resolving of inconsistencies)
            var result = _containers.ExecuteAndResolvePossibleIncosistencies(RegistrationFunction);

            // Cache the result using all ids
            foreach (var configurationObject in constructedObjects)
            {
                // Pull id
                var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set.");

                // Cache the result for the object
                _cachedResults.Add(id, result);
            }

            // If the result is not ok, we can directly return it
            if (result != RegistrationResult.Ok)
                return result;

            // Otherwise we have to register the default theorems
            foreach (var theorem in defaultTheorems)
            {
                _theoremsContainer.Add(theorem);
            }

            // And finally we can return the result (which should be OK in this case)
            return result;
        }

        /// <summary>
        /// Performs the actual registration of given objects into all objects containers and
        /// also sets the default theorem in case when the registration result is OK.
        /// </summary>
        /// <param name="objects">The objects to be constructed.</param>
        /// <param name="theorems">The default theorems list.</param>
        /// <returns>The result of the registration.</returns>
        private RegistrationResult RegisterObjects(List<ConstructedConfigurationObject> objects, out List<Theorem> theorems)
        {
            // Initialize a variable indicating if the construction is possible
            bool? canBeConstructed = null;

            // Initialize a list of objects duplicate objects
            List<ConfigurationObject> duplicates = null;

            // Perform the construction
            var constructorOutput = _resolver.Resolve(objects[0].Construction).Construct(objects);

            // Add objects to all containers
            foreach (var container in _containers)
            {
                // Add objects
                var containerResult = container.Add(objects, constructorOutput.ConstructorFunction);

                // If they are null, that means the construction failed in this container
                if (containerResult == null)
                {
                    // However if some container marked this objects as constructible,
                    // then we have inconsistency
                    if (canBeConstructed != null && canBeConstructed.Value)
                        throw new InconsistentContainersException();

                    // Otherwise we can mark the object as not constructible
                    canBeConstructed = false;

                    // And move to the next container
                    continue;
                }

                // If we got here, the construction is possible
                canBeConstructed = true;

                // If duplicates have been already set and they are not equal to the
                // current container result, then we have inconsistency
                if (duplicates != null && !duplicates.SequenceEqual(containerResult))
                    throw new InconsistentContainersException();

                // Otherwise we can mark the current duplicates
                duplicates = containerResult;
            }

            // If we can't construct it
            if (!canBeConstructed ?? throw new AnalyzerException("Impossible"))
            {
                // Then we don't need to evaluate the theorems
                theorems = null;

                // And return unconstructible result
                return RegistrationResult.Unconstructible;
            }

            // If there are duplicates
            if (!duplicates.SequenceEqual(objects))
            {
                // Then we don't need to evaluate the theorems as well
                theorems = null;

                // And we return the duplicates result
                return RegistrationResult.Duplicates;
            }
            
            // Otherwise we evaluate the theorems
            theorems = constructorOutput.DefaultTheoremsFunction();
            
            // And return the ok result
            return RegistrationResult.Ok;
        }

        /// <summary>
        /// Finds our whether the objects haven been already registered.
        /// </summary>
        /// <param name="objects">The objects.</param>
        /// <returns>The cashed result, if there is any; otherwise null</returns>
        private RegistrationResult? FindCachedRegistrationResult(List<ConstructedConfigurationObject> objects)
        {
            // Find object ids
            var ids = objects.Select(obj => obj.Id ?? throw new AnalyzerException("Id must be set")).ToList();

            // Find the number of resolved ids
            var resolvedIds = ids.Count(id => _cachedResults.Keys.Contains(id));

            // If they match, then we're sure that objects have been resolved
            // and we pull the result using id of one of them
            if (resolvedIds == objects.Count)
                return _cachedResults[ids[0]];

            // If there is no resolved id, return null
            if (resolvedIds == 0)
                return null;

            // Otherwise there is an internal problem...
            throw new AnalyzerException("Some objects from the same construction are present and other are not.");
        }

        #endregion
    }
}