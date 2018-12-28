using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// The default implementation of <see cref="IGeometryRegistrar"/>. 
    /// </summary>
    public class GeometryRegistrar : IGeometryRegistrar
    {
        #region Dependencies

        /// <summary>
        /// The factory for creating object container managers that hold the actual geometry.
        /// </summary>
        private readonly IObjectsContainersManagerFactory _factory;

        /// <summary>
        /// The constructor of loose objects.
        /// </summary>
        private readonly ILooseObjectsConstructor _constructor;

        /// <summary>
        /// The resolver of object constructors for particular constructions.
        /// </summary>
        private readonly IConstructorsResolver _resolver;

        #endregion

        #region Private fields

        /// <summary>
        /// The dictionary where we store object managers for successfully registered configurations.
        /// </summary>
        private readonly Dictionary<Configuration, IObjectsContainersManager> _managers = new Dictionary<Configuration, IObjectsContainersManager>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryRegistrar"/> class.
        /// </summary>
        /// <param name="factory">The factory for creating object container managers that hold the actual geometry.</param>
        /// <param name="constructor">The constructor of loose objects.</param>
        /// <param name="resolver">The resolver of object constructors for particular constructions.</param>
        public GeometryRegistrar(IObjectsContainersManagerFactory factory, ILooseObjectsConstructor constructor, IConstructorsResolver resolver)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        #endregion

        #region IGeometryRegistrar implementation

        /// <summary>
        /// Registers a given configuration to the geometry system. The configuration must be constructible 
        /// and must not contain duplicate objects. If it does, the registration won't be successful, i.e. 
        /// the geometrical representation of the configuration won't be stored.
        /// </summary>
        /// <param name="configuration">The configuration to be registered.</param>
        /// <returns>A registration result containing information about inconstructible and duplicate objects.</returns>
        public RegistrationResult Register(Configuration configuration)
        {
            // Create the manager for the configuration
            var manager = _factory.CreateContainersManager();

            // First we add loose objects to all containers
            foreach (var container in manager)
            {
                // Objects are constructed using the injected loose objects constructed
                container.Add(configuration.LooseObjectsHolder.LooseObjects, () => _constructor.Construct(configuration.LooseObjectsHolder));
            }

            // Then we also add all the constructed object
            foreach (var constructedObject in configuration.ConstructedObjects)
            {
                // For a given one prepare a function that performs the actual registration of it
                RegistrationResult RegistrationFunction() => RegisterObject(constructedObject, manager);

                // Safely execute the registration (with auto-resolving of inconsistencies)
                var result = manager.ExecuteAndResolvePossibleIncosistencies(RegistrationFunction);

                // Find out if the result is correct
                var correctResult = result.InconstructibleObject == null && result.Duplicates == (null, null);

                // If it's not, we directly return the result without dealing with the remaining objects
                if (!correctResult)
                    return result;
            }

            // If we got here, then all the objects have been registered correctly 
            // We may try to cache the manager
            try
            {
                _managers.Add(configuration, manager);
            }
            // If we couldn't, it means the configuration has already been registered
            catch (ArgumentException)
            {
                throw new AnalyzerException("The configuration is already registered.");
            }

            // And finally we can return an empty (i.e. OK) result
            return new RegistrationResult();
        }

        /// <summary>
        /// Gets the manager corresponding to a given configuration. The configuration must
        /// have been registered before.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The manager corresponding to the given configuration.</returns>
        public IObjectsContainersManager GetContainersManager(Configuration configuration)
        {
            // Try to get the manager from the cache
            try
            {
                return _managers[configuration];
            }
            // If we couldn't, then the configuration hasn't been registered yet
            catch (KeyNotFoundException)
            {
                throw new AnalyzerException("The configuration hasn't been cached yet");
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Performs the actual registration of a given object into all the objects containers of a given containers manager.
        /// </summary>
        /// <param name="configurationObject">The object to be registered.</param>
        /// <param name="manager">The manager of all the objects containers where we should add the object.</param>
        /// <returns>The result of the registration.</returns>
        private RegistrationResult RegisterObject(ConstructedConfigurationObject configurationObject, IObjectsContainersManager manager)
        {
            // Initialize a variable indicating if the construction is possible
            bool canBeConstructed = default;

            // Initialize a variable holding the duplicate version of the object
            ConfigurationObject duplicate = default;

            // Initialize a variable indicating if we've already processed some container
            var noContainerProcessed = true;

            // Let the resolver find the constructor and let it create the constructor function
            var constructorFunction = _resolver.Resolve(configurationObject.Construction).Construct(configurationObject);

            // Add the object to all the containers
            foreach (var container in manager)
            {
                // Add the object to the current one using this function that uses the current container
                container.TryAdd(configurationObject, () => constructorFunction(container), out var objectConstructed, out var equalObject);

                // We need to first check if some other container didn't mark constructibility in the opposite way
                // If yes, we have an inconsistency
                if (!noContainerProcessed && canBeConstructed != objectConstructed)
                    throw new InconsistentContainersException();

                // Now we need to check if some other container didn't find a different duplicate 
                // If yes, we have an inconsistency
                if (!noContainerProcessed && duplicate != equalObject)
                    throw new InconsistentContainersException();

                // If this is the first container that we're processing...
                if (noContainerProcessed)
                {
                    // Then we need to set the constructible and duplicate variables
                    // so we can work with them in the other iterations
                    canBeConstructed = objectConstructed;
                    duplicate = equalObject;

                    // And set that we've processed a container
                    noContainerProcessed = false;
                }
            }

            //  Now all the objects are added to all the containers, and we can return the result
            return new RegistrationResult
            {
                // Set inconstructible object to the provided object, if it can't be constructed
                InconstructibleObject = !canBeConstructed ? configurationObject : default,

                // Set the duplicates to the pair of this object and the found duplicate, if there's any
                Duplicates = duplicate != null ? (olderObject: duplicate, newerObject: configurationObject) : default
            };
        }

        #endregion
    }
}