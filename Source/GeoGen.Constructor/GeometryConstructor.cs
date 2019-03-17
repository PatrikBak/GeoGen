using GeoGen.Core;
using System;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The default implementation of <see cref="IGeometryConstructor"/>. 
    /// </summary>
    public class GeometryConstructor : IGeometryConstructor
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

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryConstructor"/> class.
        /// </summary>
        /// <param name="factory">The factory for creating object container managers that hold the actual geometry.</param>
        /// <param name="constructor">The constructor of loose objects.</param>
        /// <param name="resolver">The resolver of object constructors for particular constructions.</param>
        public GeometryConstructor(IObjectsContainersManagerFactory factory, ILooseObjectsConstructor constructor, IConstructorsResolver resolver)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        #endregion

        #region IGeometryConstructor implementation

        /// <summary>
        /// Constructs a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration to be constructed.</param>
        /// <returns>The geometry data of the configuration.</returns>
        public GeometryData Construct(Configuration configuration)
        {
            // Create a manager for the configuration
            var manager = _factory.CreateContainersManager();

            // First we add loose objects to all containers
            foreach (var container in manager)
            {
                // Objects are constructed using the loose objects constructor
                container.Add(configuration.LooseObjectsHolder.LooseObjects, () => _constructor.Construct(configuration.LooseObjectsHolder));
            }

            // Then we add all the constructed object
            foreach (var constructedObject in configuration.ConstructedObjects)
            {
                // Prepare the variable holding the final result
                GeometryData data;

                try
                {
                    // Execute the construction using our helper function
                    data = manager.ExecuteAndResolvePossibleIncosistencies(() => ConstructObject(constructedObject, manager, addToContainers: true));
                }
                // If there are unresolvable inconsistencies....
                catch (UnresolvableInconsistencyException)
                {
                    // Then we can't do more...
                    return new GeometryData { SuccessfullyExamined = false };
                }

                // At this point the construction of the object is completed
                // Find out if the result is correct
                var correctResult = data.InconstructibleObject == null && data.Duplicates == (null, null);

                // If it's not, we directly return the current data without dealing with the remaining objects
                if (!correctResult)
                    return data;
            }

            // If we got here, then there are no inconstructible objects and no duplicates
            return new GeometryData
            {
                // Set that we drew it
                SuccessfullyExamined = true,

                // Set the manager
                Manager = manager
            };
        }

        /// <summary>
        /// Performs geometric examination of a given constructed object with respect to a given containers manager.
        /// The object will be constructed, but won't be added to manager's containers.
        /// </summary>
        /// <param name="constructedObject">The constructed configuration object to be examined.</param>
        /// <param name="manager">The manager of objects containers.</param>
        /// <returns>The geometry data of the object.</returns>
        public GeometryData Examine(ConstructedConfigurationObject constructedObject, IObjectsContainersManager manager)
        {
            try
            {
                // Execute the construction without adding the object to the container
                return manager.ExecuteAndResolvePossibleIncosistencies(() => ConstructObject(constructedObject, manager, addToContainers: false));
            }
            // If there are unresolvable inconsistencies...
            catch (UnresolvableInconsistencyException)
            {
                // Then we can't do more...
                return new GeometryData { SuccessfullyExamined = false };
            }
        }


        #endregion

        #region Private methods

        /// <summary>
        /// Performs the construction of a given constructed object with respect to all the containers of a given manager.
        /// </summary>
        /// <param name="constructedObject">The object to be constructed.</param>
        /// <param name="manager">The manager of all the objects containers.</param>
        /// <param name="addToContainers">Indicates if the object should be added to the containers.</param>
        /// <returns>The result of the construction.</returns>
        private GeometryData ConstructObject(ConstructedConfigurationObject constructedObject, IObjectsContainersManager manager, bool addToContainers)
        {
            // Initialize a variable indicating if the construction is possible
            bool canBeConstructed = default;

            // Initialize a variable holding a potential duplicate version of the object
            ConfigurationObject duplicate = default;

            // Initialize a variable indicating if we've already processed some container
            var noContainerProcessed = true;

            // Let the resolver find the constructor and let it create the constructor function
            var constructorFunction = _resolver.Resolve(constructedObject.Construction).Construct(constructedObject);

            // Add the object to all the containers
            foreach (var container in manager)
            {
                // Prepare value indicating whether the object was constructed in the container
                var objectConstructed = default(bool);

                // Prepare value holding a potential equal object in the container to this object
                var equalObject = default(ConfigurationObject);

                // If we are supposed to add the object...
                if (addToContainers)
                {
                    // Then ask the container to do it (it will perform the construction)
                    container.TryAdd(constructedObject, () => constructorFunction(container), out objectConstructed, out equalObject);
                }
                // Otherwise we need to perform the construction here
                else
                {
                    // Construct it
                    var analyticObject = constructorFunction(container);

                    // Set if it the construction went fine
                    objectConstructed = analyticObject != null;

                    // Set if there is an equal object
                    equalObject = analyticObject != null && container.Contains(analyticObject) ? container.Get(analyticObject) : null;
                }

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

            //  Now the object is handled with respect to all the containers
            return new GeometryData
            {
                // Set that the object was examined correctly
                SuccessfullyExamined = true,

                // Set the inconstructible object to the given one, if it can't be constructed
                InconstructibleObject = !canBeConstructed ? constructedObject : default,

                // Set the duplicates to the pair of this object and the found duplicate, if there's any
                Duplicates = duplicate != null ? (olderObject: duplicate, newerObject: constructedObject) : default,

                // Set the manager
                Manager = manager
            };
        }

        #endregion
    }
}