using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The default implementation of <see cref="IObjectsContainer"/>.
    /// </summary>
    public class ObjectsContainer : IObjectsContainer
    {
        #region Private fields

        /// <summary>
        /// The map representing the actual content of the container.
        /// </summary>
        private readonly Map<ConfigurationObject, IAnalyticObject> _content = new Map<ConfigurationObject, IAnalyticObject>();

        /// <summary>
        /// The dictionary mapping accepted types of analytic objects to 
        /// their corresponding configuration object types.
        /// </summary>
        private readonly IReadOnlyDictionary<Type, ConfigurationObjectType> _correctTypes;

        /// <summary>
        /// The list of cached constructor functions. Each of them performs a construction of some
        /// object(s) and returns whether it was successful.
        /// </summary>
        private readonly List<Func<bool>> _reconstructors = new List<Func<bool>>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectsContainer"/> class.
        /// </summary>
        public ObjectsContainer()
        {
            // Set the correct types mapping
            _correctTypes = new Dictionary<Type, ConfigurationObjectType>
            {
                {typeof(Point), ConfigurationObjectType.Point},
                {typeof(Circle), ConfigurationObjectType.Circle},
                {typeof(Line), ConfigurationObjectType.Line}
            };
        }

        #endregion

        #region IObjectsContainer implementation

        /// <summary>
        /// Adds given configuration objects to the container. Their analytic versions are constructed
        /// using a provided constructor function. This method should be used only when we're sure that
        /// the construction will succeed (for example, for constructing loose objects).
        /// </summary>
        /// <param name="objects">The configuration objects to be added to the container.</param>
        /// <param name="constructor">The constructor function that performs the construction of the analytic versions of the objects.</param>
        public void Add(IEnumerable<ConfigurationObject> objects, Func<List<IAnalyticObject>> constructor)
        {
            // Prepare the constructor function
            bool Construct()
            {
                // We expect this construct to succeed
                try
                {
                    // Perform the construction
                    var analyticObjects = constructor();

                    // Add them one by one
                    objects.ForEach((configurationObject, index) =>
                    {
                        // Add the current one
                        TryAdd(configurationObject, analyticObjects[index], out var equalObject);

                        // If there is an equal object, we have a problem...
                        if (equalObject != null)
                            throw new AnalyticException("Couldn't add objects to the container because of the duplicity.");
                    });

                    // If we got here, the construction went fine after all
                    return true;
                }
                // If it doesn't, we have a problem...
                catch (Exception e)
                {
                    throw new ConstructorException("An attempt to add configuration objects whose construction failed", e);
                }
            }

            // Cache it
            _reconstructors.Add(Construct);

            // Perform it
            Construct();
        }

        /// <summary>
        /// Tries to add a given configuration object to the container. The analytic version of the object is 
        /// constructed using the provided constructor function. The method sets the out parameters indicating
        /// whether the construction was successful and whether there is a duplicate version of the object.
        /// The object is added if and only if the construction is successful and there is no duplicity.
        /// </summary>
        /// <param name="configurationObject">The configuration object to be added to the container.</param>
        /// <param name="constructor">The constructor function that performs the construction of the analytic version of the object.</param>
        /// <param name="equalObject">If there already is the same object in the container, this value will be set to that object. Otherwise it will be null.</param>
        /// <param name="objectConstructed">Indicates if the construction was successful.</param>
        public void TryAdd(ConfigurationObject configurationObject, Func<IAnalyticObject> constructor, out bool objectConstructed, out ConfigurationObject equalObject)
        {
            // Local function that does the construction and returns whether it's constructible and possibly an equal object
            (bool successful, ConfigurationObject equalObject) Construct()
            {
                // Execute the constructor
                var analyticObject = constructor();

                // If it's null, the construction's is unsuccessful
                if (analyticObject == null)
                    return (successful: false, equalObject: null);

                // Otherwise we'll try to add the objects
                TryAdd(configurationObject, analyticObject, out var duplicitObject);

                // Construct the result
                return (successful: true, equalObject: duplicitObject);
            }

            // Cache the functions that constructs the result and returns success only if the construction
            // was successful and there was no equal object
            _reconstructors.Add(() => Construct() == (successful: true, equalObject: null));

            // Perform the construction for the first time
            var (successful, sameObject) = Construct();

            // Set the out parameters
            objectConstructed = successful;
            equalObject = sameObject;
        }

        /// <summary>
        /// Gets the analytic representation of a given configuration object. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The analytic object of a given configuration object.</returns>
        public IAnalyticObject Get(ConfigurationObject configurationObject) => _content.GetRightValue(configurationObject);

        /// <summary>
        /// Gets the configuration object corresponding to a given analytic object.
        /// </summary>
        /// <param name="analyticObject">The analytic object.</param>
        /// <returns>The configuration object of a given analytic object.</returns>
        public ConfigurationObject Get(IAnalyticObject analyticObject) => _content.GetLeftValue(analyticObject);

        /// <summary>
        /// Finds out if a given analytic object is present if the container.
        /// </summary>
        /// <param name="analyticObject">The analytic object.</param>
        /// <returns>true, if the object is present in the container; false otherwise.</returns>
        public bool Contains(IAnalyticObject analyticObject) => _content.ContainsRightKey(analyticObject);

        /// <summary>
        /// Finds out if a given configuration object is present if the container.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>true, if the object is present in the container; false otherwise.</returns>
        public bool Contains(ConfigurationObject configurationObject) => _content.ContainsLeftKey(configurationObject);

        /// <summary>
        /// Tries to reconstruct all the objects in the container. If the reconstruction fails, 
        /// the content of the container will remain unchanged.
        /// </summary>
        /// <param name="reconstructionSuccessful">true, if the reconstruction was successful; false otherwise.</param>
        public void TryReconstruct(out bool reconstructionSuccessful)
        {
            // Cache the content so we can revert it if the reconstruction fails
            var cachedContent = _content.ToList();

            // Clear the content 
            _content.Clear();

            // Go through all the reconstructors...
            foreach (var reconstructor in _reconstructors)
            {
                // Perform a given one
                var currentResult = reconstructor();

                // If the reconstruction wasn't successful...
                if (!currentResult)
                {
                    // We mark it
                    reconstructionSuccessful = false;

                    // Revert the content
                    _content.SetItems(cachedContent);

                    // And stop
                    return;
                }
            }

            // If we got here, the reconstruction went fine
            reconstructionSuccessful = true;
        }

        #endregion

        #region IEnumerable implementation

        /// <summary>
        /// Gets a generic enumerator.
        /// </summary>
        /// <returns>A generic enumerator.</returns>
        public IEnumerator<(ConfigurationObject configurationObject, IAnalyticObject analyticObject)> GetEnumerator() => _content.GetEnumerator();

        /// <summary>
        /// Gets a non-generic enumerator.
        /// </summary>
        /// <returns>A non-generic enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Private methods

        /// <summary>
        /// Tries to add given pair of objects to the container. If there already is 
        /// an equal analytic object, the <paramref name="equalObject"/> will be set to
        /// and the object won't be added. Otherwise it will be added and the 
        /// <paramref name="equalObject"/> will have its default value (which is null).
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <param name="analyticalObject">The analytical object.</param>
        /// <param name="equalObject">Either the equal configuration object already present in the container; or null, if there's none.</param>
        private void TryAdd(ConfigurationObject configurationObject, IAnalyticObject analyticObject, out ConfigurationObject equalObject)
        {
            // Check if the types of objects correspond. This could save us some time while finding weird errors
            if (_correctTypes[analyticObject.GetType()] != configurationObject.ObjectType)
                throw new ConstructorException("Can't add objects of wrong types to the container.");

            // Check if we don't have a duplicity 
            if (_content.ContainsRightKey(analyticObject))
            {
                // If yes, set it
                equalObject = _content.GetLeftValue(analyticObject);

                // And terminate
                return;
            }

            // Now we can safely add them 
            _content.Add(configurationObject, analyticObject);

            // And set that there is not equal object
            equalObject = default;
        }

        #endregion
    }
}