using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IObjectsContainer"/>.
    /// </summary>
    internal class ObjectsContainer : IObjectsContainer
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping analytical objects to their corresponding
        /// configuration objects.
        /// </summary>
        private readonly Dictionary<AnalyticalObject, ConfigurationObject> _objectsDictionary;

        /// <summary>
        /// The dictionary mapping configuration object's ids 
        /// </summary>
        private readonly Dictionary<int, AnalyticalObject> _idToObjects;

        /// <summary>
        /// The dictionary mapping accepted types of analytical objects to 
        /// their corresponding configuration object types.
        /// </summary>
        private readonly IReadOnlyDictionary<Type, ConfigurationObjectType> _correctTypes;

        /// <summary>
        /// The list of cached constructor functions. Each of them performs a construction of some
        /// object(s) and returns whether it was successful.
        /// </summary>
        private readonly List<Func<bool>> _reconstructors = new List<Func<bool>>();

        /// <summary>
        /// The tracker of possible inconsistencies regarding failed attempts to re-initialize
        /// the container.
        /// </summary>
        private readonly IInconsistenciesTracker _tracker;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="tracker">The tracker for marking unsuccessful attempts to reconstruct the container.</param>
        public ObjectsContainer(IInconsistenciesTracker tracker = null)
        {
            _tracker = tracker;
            _objectsDictionary = new Dictionary<AnalyticalObject, ConfigurationObject>();
            _idToObjects = new Dictionary<int, AnalyticalObject>();
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
        /// Adds given objects to the container. The analytic versions of these objects
        /// will be constructed using a provided constructor function. This function is
        /// important to give the container an ability to reconstruct itself. This method
        /// returns either null, when the construction can't be performed, or a list
        /// of configuration objects. In this list, every configuration objects
        /// corresponds to the object in the provided objects enumerable. If the 
        /// analytical version of the object is already present in the container, 
        /// then these objects will be the same, otherwise the object in the list will
        /// be the one that representation the duplicate object.
        /// </summary>
        /// <param name="objects">The analytical objects to be constructed.</param>
        /// <param name="constructor">The function that performs the construction.</param>
        /// <returns>null, if the construction failed; or the representation of equal objects from the container.</returns>
        public List<ConfigurationObject> Add(IEnumerable<ConfigurationObject> objects, Func<IObjectsContainer, List<AnalyticalObject>> constructor)
        {
            // Enumerate the objects
            var objectsList = objects.ToList();

            // Prepare local function that performs the construction and returns the output
            List<ConfigurationObject> Construct()
            {
                // Perform construction to obtain the analytical objects
                var analytical = constructor(this);

                // If there are null, the construction has failed
                if (analytical is null)
                    return null;

                // Otherwise add all gotten objects and return the results of the Add method
                // (that returns either the same object, if the analytical object is not present, or 
                // the configuration object corresponding to the duplicate version of the analytical object)
                return objectsList.Select((o, i) => Add(analytical[i], o)).ToList();
            }

            // Prepare local function that finds out if a given result of the Construct 
            // function is correct
            bool IsConstructorOutputCorrect(List<ConfigurationObject> output)
            {
                // Return whether the construction is possible and yields non-duplicate objects
                return output != null && output.SequenceEqual(objectsList);
            }

            // Apply the constructor function
            var currentResult = Construct();

            // If the construction result is fine
            if (IsConstructorOutputCorrect(currentResult))
            {
                // Cache the constructor so we can invoke it while reconstructing the container
                _reconstructors.Add(() => IsConstructorOutputCorrect(Construct()));
            }

            // Return the result in every case
            return currentResult;
        }

        /// <summary>
        /// Gets the analytical representation of a given configuration object. 
        /// </summary>
        /// <typeparam name="T">The type of analytical object.</typeparam>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The analytical object.</returns>
        public T Get<T>(ConfigurationObject configurationObject) where T : AnalyticalObject
        {
            // Pull id
            var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set.");

            try
            {
                // Try to get the result from the dictionary. This might throw an KeyNotFoundException
                var result = _idToObjects[id];

                // Try to cast the result to the requested type
                if (!(result is T castedResult))
                    throw new AnalyzerException("Incorrect asked type of the analytical object.");

                // And return it
                return castedResult;
            }
            catch (KeyNotFoundException)
            {
                throw new AnalyzerException("Object not found in the container.");
            }
        }

        /// <summary>
        /// Gets the analytical representation of a given configuration object. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The analytical object.</returns>
        public AnalyticalObject Get(ConfigurationObject configurationObject)
        {
            // Little hack to utilize the other Get method
            return Get<AnalyticalObject>(configurationObject);
        }

        /// <summary>
        /// Reconstructs all objects in the container. In general, it might happen that
        /// the reconstruction fails (not all objects will be constructible). This method
        /// will try to perform the reconstruction until it's successful. 
        /// </summary>
        public void Reconstruct()
        {
            // Try to reconstruct until it's successful
            while (true)
            {
                // Clear the dictionaries that hold all objects
                _idToObjects.Clear();
                _objectsDictionary.Clear();

                // Prepare a variable that indicates whether the reconstruction 
                // was successful
                var successful = true;

                // Execute all constructors from the constructors list
                foreach (var reconstructor in _reconstructors)
                {
                    // If the construction is successful, move on
                    if (reconstructor())
                        continue;

                    // Otherwise update the flag
                    successful = false;

                    // Track an unsuccessful attempt to reconstruct the container
                    _tracker?.OnUnsuccessfulAttemptToReconstructContainer();

                    // Break the loop
                    break;
                }

                // If the reconstruction was successful, we're done
                if (successful)
                    break;
            }
        }

        /// <summary>
        /// Adds a given object to the container. If the analytical version 
        /// of the object is already present in the container, then it will return
        /// the instance the <see cref="ConfigurationObject"/> that represents the 
        /// given object. If the object is new, it will return the original object.
        /// </summary>
        /// <param name="analyticalObject">The analytical object.</param>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The representation of an equal object.</returns>
        private ConfigurationObject Add(AnalyticalObject analyticalObject, ConfigurationObject configurationObject)
        {
            // Check if the types of objects correspond. This could save us some time while finding an error like this
            if (_correctTypes[analyticalObject.GetType()] != configurationObject.ObjectType)
                throw new AnalyzerException("Can't add objects of wrong types to the container.");

            // If the object is in the dictionary, return its configuration object version
            if (_objectsDictionary.ContainsKey(analyticalObject))
                return _objectsDictionary[analyticalObject];

            // Otherwise add the object to the dictionary
            _objectsDictionary.Add(analyticalObject, configurationObject);

            // Pull id
            var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set");

            // And also add it to the id to object dictionary
            _idToObjects.Add(id, analyticalObject);

            // And return the object that was passed
            return configurationObject;
        }

        #endregion
    }
}