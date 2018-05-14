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
        /// The constructor of loose objects.
        /// </summary>
        private readonly ILooseObjectsConstructor _constructor;

        /// <summary>
        /// The resolver for finding constructors.
        /// </summary>
        private readonly IConstructorsResolver _resolver;

        /// <summary>
        /// The container for keeping default (trivial) theorems.
        /// </summary>
        private readonly ITheoremsContainer _container;

        /// <summary>
        /// The mapper that maps configurations to their geometrical representations
        /// wrapped inside a <see cref="IObjectsContainersManager"/>.
        /// </summary>
        private readonly IObjectContainersMapper _mapper;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="resolver">The resolver for finding the right constructors.</param>
        /// <param name="constructor">The constructor of loose objects.</param>
        /// <param name="container">The container for keeping default theorems.</param>
        /// <param name="mapper">The manager of all objects container where we register the objects.</param>
        public GeometryRegistrar(IConstructorsResolver resolver, ILooseObjectsConstructor constructor, ITheoremsContainer container, IObjectContainersMapper mapper)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #endregion

        #region IGeometryRegistrar implementation

        /// <summary>
        /// Registers a given configuration to the actual geometrical system
        /// and returns if it is contructible and if it doesn't contain
        /// duplicate objects.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The registration result.</returns>
        public RegistrationResult Register(Configuration configuration)
        {
            // Create the manager for the configuration
            var manager = _mapper.Create(configuration);

            // Prepare theorems list. It will be filled by the RegisterObjects method
            var theorems = new List<Theorem>();

            // First we construct loose objects to all containers
            foreach (var container in manager)
            {
                // Objects are constructed using the injected loose objects constructed
                container.Add(configuration.LooseObjects, c => _constructor.Construct(configuration.LooseObjectsHolder));
            }
            
            // Then we group the constructed objects into constructible groups 
            foreach (var constructedObjects in configuration.GroupConstructedObjects())
            {
                // Prepare a function that performs the actual registration of them
                RegistrationResult RegistrationFunction() => RegisterObjects(constructedObjects, theorems, manager);

                // Safely execute the registration (with auto-resolving of inconsistencies)
                var result = manager.ExecuteAndResolvePossibleIncosistencies(RegistrationFunction);

                // Find out if the result is correct
                var correctResult = result.UnconstructibleObjects == null && result.Duplicates == null;

                // If it's not, we may directly return this result. We don't care if the other objects
                // are constructible. That is not a big deal, because if this service is running during
                // the standard generation process, then the configuration shouldn't have more than one
                // incorrect group of constructed objects.
                if (!correctResult)
                    return result;
            }

            // If we got here, then all the objects have been registered correctly
            
            // We may add the default theorems
            foreach (var theorem in theorems)
            {
                _container.Add(theorem);
            }

            // And finally return the OK result
            return new RegistrationResult
            {
                UnconstructibleObjects = null,
                Duplicates = null
            };
        }

        /// <summary>
        /// Performs the actual registration of given objects into all objects containers and
        /// also fill the default theorems list in case when the registration result is OK.
        /// </summary>
        /// <param name="objects">The objects to be constructed, the result of a single construction.</param>
        /// <param name="theorems">The default theorems list.</param>
        /// <param name="manager">The manager of all objects containers to be considered.</param>
        /// <returns>The result of the registration.</returns>
        private RegistrationResult RegisterObjects(List<ConstructedConfigurationObject> objects, List<Theorem> theorems, IObjectsContainersManager manager)
        {
            // Initialize a variable indicating if the construction is possible
            bool? canBeConstructed = null;

            // Initialize a list of objects duplicate objects
            List<ConfigurationObject> duplicates = null;

            // Perform the construction
            var constructorOutput = _resolver.Resolve(objects[0].Construction).Construct(objects);

            // Add the objects to all containers
            foreach (var container in manager)
            {
                // Add the objects
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
            if (!canBeConstructed.Value)
            {
                // Return corresponding result
                return new RegistrationResult
                {
                    UnconstructibleObjects = objects,
                    Duplicates = null
                };
            }

            // Prepare duplicates list (might be empty)
            // Take all needed indices
            var duplicatesList = Enumerable.Range(0, duplicates.Count)
                    // Take only those that represent non-matching objects
                    .Where(i => duplicates[i] != objects[i])
                    // Cast these objects to a tuple
                    .Select(i => ((ConfigurationObject)objects[i], duplicates[i]))
                    // And enumerate to a list
                    .ToList();

            // If there is any duplicate
            if (duplicatesList.Any())
            {
                // Return corresponding result
                return new RegistrationResult
                {
                    UnconstructibleObjects = null,
                    Duplicates = duplicatesList
                };
            }
            
            // Otherwise we evaluate the theorms and add them to the prepares list
            theorems.AddRange(constructorOutput.DefaultTheoremsFunction());
            
            // And return the ok result
            return new RegistrationResult
            {
                UnconstructibleObjects = null,
                Duplicates = null
            };
        }

        #endregion
    }
}