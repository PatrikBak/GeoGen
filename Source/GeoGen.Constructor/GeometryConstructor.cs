using GeoGen.Core;
using System;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The default implementation of <see cref="IGeometryConstructor"/>. 
    /// </summary>
    public class GeometryConstructor : IGeometryConstructor
    {
        #region Dependencies

        /// <summary>
        /// The factory for creating picture managers that hold the actual geometry.
        /// </summary>
        private readonly IPicturesManagerFactory _factory;

        /// <summary>
        /// The constructor of loose objects.
        /// </summary>
        private readonly ILooseObjectsConstructor _constructor;

        /// <summary>
        /// The resolver of object constructors for particular constructions.
        /// </summary>
        private readonly IConstructorsResolver _resolver;

        /// <summary>
        /// The tracer for objects that couldn't be constructed because of inconsistencies between pictures.
        /// </summary>
        private readonly IGeometryConstructionFailureTracer _tracer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryConstructor"/> class.
        /// </summary>
        /// <param name="factory">The factory for creating picture managers that hold the actual geometry.</param>
        /// <param name="constructor">The constructor of loose objects.</param>
        /// <param name="resolver">The resolver of object constructors for particular constructions.</param>
        /// <param name="tracer">The tracer for objects that couldn't be constructed because of inconsistencies between pictures.</param>
        public GeometryConstructor(IPicturesManagerFactory factory, ILooseObjectsConstructor constructor, IConstructorsResolver resolver, IGeometryConstructionFailureTracer tracer = null)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _tracer = tracer;
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
            var manager = _factory.CreatePicturesManager();

            // First we add loose objects to all pictures
            foreach (var pictures in manager)
            {
                // Objects are constructed using the loose objects constructor
                pictures.Add(configuration.LooseObjectsHolder.LooseObjects, () => _constructor.Construct(configuration.LooseObjectsHolder));
            }

            // Then we add all the constructed object
            foreach (var constructedObject in configuration.ConstructedObjects)
            {
                // Prepare the variable holding the final result
                GeometryData data = null;

                try
                {
                    // Execute the construction using our helper function
                    manager.ExecuteAndResolvePossibleIncosistencies(
                        // Call the internal construction function
                        () => data = ConstructObject(constructedObject, manager, addToPictures: true),
                        // Trace any inconsistency exception
                        e => _tracer?.TraceInconsistencyWhileDrawingConfiguration(configuration, constructedObject, e.Message));
                }
                // If there are unresolvable inconsistencies...
                catch (UnresolvedInconsistencyException e)
                {
                    // We trace it
                    _tracer?.TraceUnresolvedInconsistencyWhileDrawingConfiguration(configuration, e.Message);

                    // And return failure
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
        /// Performs geometric examination of a given constructed object with respect to a given pictures manager
        /// that represents a given configuration.
        /// The object will be constructed, but won't be added to the manager's pictures.
        /// </summary>
        /// <param name="configuration">The configuration that is drawn in the manager's pictures.</param>
        /// <param name="manager">The manager of pictures where all the needed objects for the constructed object should be drawn.</param>
        /// <param name="constructedObject">The constructed configuration object to be examined.</param>
        /// <returns>The geometry data of the object.</returns>
        public GeometryData Examine(Configuration configuration, IPicturesManager manager, ConstructedConfigurationObject constructedObject)
        {
            try
            {
                // Prepare the result
                var data = default(GeometryData);

                // Execute the construction without adding the object to the picture
                manager.ExecuteAndResolvePossibleIncosistencies(
                    // Call the internal construction function
                    () => data = ConstructObject(constructedObject, manager, addToPictures: false),
                    // Trace any inconsistency exception
                    e => _tracer?.TraceInconsistencyWhileExaminingObject(configuration, constructedObject, e.Message));

                // Return the data
                return data;
            }
            // If there are unresolvable inconsistencies...
            catch (UnresolvedInconsistencyException e)
            {
                // We trace it
                _tracer?.TraceUnresolvedInconsistencyWhileExaminingObject(configuration, constructedObject, e.Message);

                // And return failure
                return new GeometryData { SuccessfullyExamined = false };
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Performs the construction of a given constructed object with respect to all the pictures of a given manager.
        /// </summary>
        /// <param name="constructedObject">The object to be constructed.</param>
        /// <param name="manager">The manager of all the pictures.</param>
        /// <param name="addToPictures">Indicates if the object should be added to the pictures.</param>
        /// <returns>The result of the construction.</returns>
        private GeometryData ConstructObject(ConstructedConfigurationObject constructedObject, IPicturesManager manager, bool addToPictures)
        {
            // Initialize a variable indicating if the construction is possible
            bool canBeConstructed = default;

            // Initialize a variable holding a potential duplicate version of the object
            ConfigurationObject duplicate = default;

            // Let the resolver find the constructor and let it create the constructor function
            var constructorFunction = _resolver.Resolve(constructedObject.Construction).Construct(constructedObject);

            // Add the object to all the pictures
            foreach (var picture in manager)
            {
                // Prepare value indicating whether the object was constructed in the picture
                var objectConstructed = default(bool);

                // Prepare value holding a potential equal object in the picture to this object
                var equalObject = default(ConfigurationObject);

                // If we are supposed to add the object...
                if (addToPictures)
                {
                    // Then ask the picture to do it (it will perform the construction)
                    picture.TryAdd(constructedObject, () => constructorFunction(picture), out objectConstructed, out equalObject);
                }
                // Otherwise we need to perform the construction here
                else
                {
                    // Construct it
                    var analyticObject = constructorFunction(picture);

                    // Set if it the construction went fine
                    objectConstructed = analyticObject != null;

                    // Set if there is an equal object
                    equalObject = analyticObject != null && picture.Contains(analyticObject) ? picture.Get(analyticObject) : null;
                }

                // We need to first check if some other picture didn't mark constructibility in the opposite way
                // If yes, we have an inconsistency
                if (picture != manager.First() && canBeConstructed != objectConstructed)
                    throw new InconsistentPicturesException("The fact whether the object can be constructed was not determined consistently.");

                // Now we need to check if some other picture didn't find a different duplicate 
                // If yes, we have an inconsistency
                if (picture != manager.First() && duplicate != equalObject)
                    throw new InconsistentPicturesException("The fact whether the object has an equal version was not determined consistently.");

                // Set the found values
                canBeConstructed = objectConstructed;
                duplicate = equalObject;
            }

            //  Now the object is handled with respect to all the pictures
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