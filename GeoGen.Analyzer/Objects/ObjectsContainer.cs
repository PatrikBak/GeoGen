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
    internal  class ObjectsContainer : IObjectsContainer
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

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ObjectsContainer()
        {
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
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            if (analyticalObject == null)
                throw new ArgumentNullException(nameof(analyticalObject));

            if (_correctTypes[analyticalObject.GetType()] != configurationObject.ObjectType)
                throw new AnalyzerException("Can't add objects of wrong types to the container.");

            if (_objectsDictionary.ContainsKey(analyticalObject))
                return _objectsDictionary[analyticalObject];

            _objectsDictionary.Add(analyticalObject, configurationObject);

            var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set");
            _idToObjects.Add(id, analyticalObject);

            return configurationObject;
        }

        public void Reconstruct()
        {
            var counter = 0;

            while (true)
            {
                _idToObjects.Clear();
                _objectsDictionary.Clear();

                var successful = true;

                foreach (var reconstructor in _reconstructors)
                {
                    if (reconstructor())
                        continue;

                    successful = false;
                    break;
                }

                counter++;

                if (successful)
                    break;
            }

            Wtf.MaximalContainerIterations = Math.Max(Wtf.MaximalContainerIterations, counter);
        }

        private readonly List<Func<bool>> _reconstructors = new List<Func<bool>>();

        public List<ConfigurationObject> Add(IEnumerable<ConfigurationObject> objects, Func<IObjectsContainer, List<AnalyticalObject>> constructor)
        {
            var objectsList = objects.ToList();

            List<ConfigurationObject> Construct()
            {
                var analytical = constructor(this);

                if (analytical is null)
                    return null;

                return objectsList.Select((o, i) => Add(analytical[i], o)).ToList();
            }

            var currentResult = Construct();

            if (currentResult != null && currentResult.SequenceEqual(objectsList))
            {
                _reconstructors.Add(() =>
                {
                    var result = Construct();

                    return result != null && objectsList.SequenceEqual(result);
                });
            }

            return currentResult;
        }

        /// <summary>
        /// Removes a given configuration object from the container. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        public void Remove(ConfigurationObject configurationObject)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set");

            if (!_idToObjects.ContainsKey(id))
                throw new AnalyzerException("Object to be removed not found in the container.");

            _objectsDictionary.Remove(_idToObjects[id]);
            _idToObjects.Remove(id);
        }

        /// <summary>
        /// Gets the analytical representation of a given configuration object. 
        /// </summary>
        /// <typeparam name="T">The type of analytical object.</typeparam>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The analytical object.</returns>
        public T Get<T>(ConfigurationObject configurationObject) where T : AnalyticalObject
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set.");

            try
            {
                var result = _idToObjects[id];

                if (!(result is T castedResult))
                    throw new AnalyzerException("Incorrect asked type of the analytical object.");

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
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            return Get<AnalyticalObject>(configurationObject);
        }

        #endregion
    }
}