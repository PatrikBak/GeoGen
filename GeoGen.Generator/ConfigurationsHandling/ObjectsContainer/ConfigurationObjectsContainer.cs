using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities.StringBasedContainer;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;

namespace GeoGen.Generator.ConfigurationsHandling.ObjectsContainer
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationObjectsContainer"/>
    /// using <see cref="StringBasedContainer{T}"/>.
    /// </summary>
    internal class ConfigurationObjectsContainer : StringBasedContainer<ConfigurationObject>, IConfigurationObjectsContainer
    {
        #region IConfigurationObjectsContainer propertis

        /// <summary>
        /// Gets the default complex configuration object to string provider that is used by the container.
        /// </summary>
        public DefaultFullObjectToStringProvider ConfigurationObjectToStringProvider { get; }

        #endregion

        #region Private fields

        /// <summary>
        /// The dictionary mapping ids of objects to actual objects.
        /// </summary>
        private readonly Dictionary<int, ConfigurationObject> _idToObjectDictionary = new Dictionary<int, ConfigurationObject>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new configuration objects container with a given
        /// object to string provider.
        /// </summary>
        /// <param name="objectToStringProvider">The object to string provider.</param>
        public ConfigurationObjectsContainer(DefaultFullObjectToStringProvider objectToStringProvider)
        {
            ConfigurationObjectToStringProvider = objectToStringProvider ?? throw new ArgumentNullException(nameof(objectToStringProvider));
        }

        #endregion

        #region StringBasedContainer abstract methods implementation

        /// <summary>
        /// Converts a given item to string.
        /// </summary>
        /// <param name="item">The given item.</param>
        /// <returns>The string representation.</returns>
        protected override string ItemToString(ConfigurationObject item)
        {
            return ConfigurationObjectToStringProvider.ConvertToString(item);
        }

        #endregion

        #region IConfigurationObjectsContainer implementation

        /// <summary>
        /// Adds a given object to a container. The object must
        /// not have set the id, it's going to be set in the container.
        /// If an equal version of the object is present in the container, 
        /// it will return instance of this internal object. Otherwise
        /// it will return this object with set id.
        /// </summary>
        /// <param name="constructedConfigurationObject">The configuration object.</param>
        /// <returns>The identified version of the configuration object.</returns>
        public ConstructedConfigurationObject Add(ConstructedConfigurationObject constructedConfigurationObject)
        {
            if (constructedConfigurationObject == null)
                throw new ArgumentNullException(nameof(constructedConfigurationObject));

            if (constructedConfigurationObject.Id.HasValue)
                throw new GeneratorException("An object must not have an id.");

            var stringRepresentation = ItemToString(constructedConfigurationObject);

            if (Items.ContainsKey(stringRepresentation))
            {
                return (ConstructedConfigurationObject) Items[stringRepresentation];
            }

            var newId = Items.Count + 1;
            constructedConfigurationObject.Id = newId;
            Items.Add(stringRepresentation, constructedConfigurationObject);

            _idToObjectDictionary.Add(newId, constructedConfigurationObject);
            ConfigurationObjectToStringProvider.CacheObject(newId, stringRepresentation);

            return constructedConfigurationObject;
        }

        /// <summary>
        /// Initializes the container with loose configuration objects.
        /// </summary>
        /// <param name="looseConfigurationObjects">The loose configuration objects enumerable.</param>
        public void Initialize(IEnumerable<LooseConfigurationObject> looseConfigurationObjects)
        {
            if (looseConfigurationObjects == null)
                throw new ArgumentNullException(nameof(looseConfigurationObjects));

            Items.Clear();
            _idToObjectDictionary.Clear();
            ConfigurationObjectToStringProvider.ClearCache();

            var counter = 1;

            foreach (var looseConfigurationObject in looseConfigurationObjects)
            {
                if (looseConfigurationObject == null)
                    throw new ArgumentException("Can't contain null values.", nameof(looseConfigurationObject));

                var id = counter++;
                looseConfigurationObject.Id = id;
                Items.Add(ConfigurationObjectToStringProvider.ConvertToString(looseConfigurationObject), looseConfigurationObject);
                _idToObjectDictionary.Add(id, looseConfigurationObject);
            }
        }

        /// <summary>
        /// Gets the object with a given id. Throws an exception, if not present.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The configuration object with the given id.</returns>
        public ConfigurationObject this[int id] => _idToObjectDictionary[id];

        #endregion
    }
}