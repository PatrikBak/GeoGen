using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities.StringBasedContainer;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;

namespace GeoGen.Generator.ConstructingConfigurations.ObjectsContainer
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationObjectsContainer"/>
    /// using <see cref="StringBasedContainer{T}"/>.
    /// </summary>
    internal class ConfigurationObjectsContainer : StringBasedContainer<ConfigurationObject>, IConfigurationObjectsContainer
    {
        /// <summary>
        /// Gets the object with a given id. Throws an exception, if not present.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The configuration object with the given id.</returns>
        public ConfigurationObject this[int id] => _idToObjectDictionary[id];

        /// <summary>
        /// Gets the default complex configuration object to string provider that is used by the container.
        /// </summary>
        private readonly DefaultFullObjectToStringProvider _objectToStringProvider;

        #region Private fields

        /// <summary>
        /// The dictionary mapping ids of objects to actual objects.
        /// </summary>
        private readonly Dictionary<int, ConfigurationObject> _idToObjectDictionary = new Dictionary<int, ConfigurationObject>();

        /// <summary>
        /// Indicates if the container has been initialized.
        /// </summary>
        private bool _initialized;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new configuration objects container with a given
        /// object to string provider and a given argument container.
        /// </summary>
        /// <param name="provider">The object to string provider.</param>
        /// <param name="container">The container.</param>
        public ConfigurationObjectsContainer(DefaultFullObjectToStringProvider provider)
        {
            _objectToStringProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        #endregion

        #region StringBasedContainer abstract methods

        /// <summary>
        /// Converts a given item to string.
        /// </summary>
        /// <param name="item">The given item.</param>
        /// <returns>The string representation.</returns>
        protected override string ItemToString(ConfigurationObject item)
        {
            return _objectToStringProvider.ConvertToString(item);
        }

        #endregion

        #region IConfigurationObjectsContainer methods

        /// <summary>
        /// Initializes the container with a given configuratin.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public void Initialize(Configuration configuration)
        {
            try
            {
                DoInitialization(configuration);
                _initialized = true;
            }
            catch (Exception)
            {
                _initialized = false;
                throw;
            }
        }

        /// <summary>
        /// Adds a given object to a container. The current id of the
        /// object will be gnored.If an equal version of the object 
        /// is present in the container, it will return instance of 
        /// this internal object. Otherwise it will return this 
        /// object with set id.
        /// </summary>
        /// <param name="constructedConfigurationObject">The configuration object.</param>
        /// <returns>The identified version of the configuration object.</returns>
        public ConstructedConfigurationObject Add(ConstructedConfigurationObject constructedConfigurationObject)
        {
            if (constructedConfigurationObject == null)
                throw new ArgumentNullException(nameof(constructedConfigurationObject));

            // Null the id
            constructedConfigurationObject.Id = null;

            // Convert the object to string
            var stringRepresentation = ItemToString(constructedConfigurationObject);

            // If we have it cached, we can return it immediately 
            if (Items.ContainsKey(stringRepresentation))
            {
                return (ConstructedConfigurationObject) Items[stringRepresentation];
            }

            // Otherwise we have a new object. Assign an id to it
            var newId = Items.Count + 1;
            constructedConfigurationObject.Id = newId;

            // Add it to the container and the id dictionary
            Items.Add(stringRepresentation, constructedConfigurationObject);
            _idToObjectDictionary.Add(newId, constructedConfigurationObject);

            // Cache the gotten string version to the provider
            _objectToStringProvider.CacheObject(newId, stringRepresentation);

            // Return the object
            return constructedConfigurationObject;
        }

        #endregion

        #region Private methods

        private void DoInitialization(Configuration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Null ids of all objects and arguments in the configuration
            NullIds(configuration);

            // Clear the container's content
            Items.Clear();

            // Clear the id dictionary
            _idToObjectDictionary.Clear();

            // Clear cache of the object to string provider
            _objectToStringProvider.ClearCache();

            // Initialize counter
            var counter = 1;

            // Add loose objects to the container
            foreach (var looseConfigurationObject in configuration.LooseObjects)
            {
                // Get new id and increase the counter
                var id = counter++;

                // Assign the id
                looseConfigurationObject.Id = id;

                // Get the string version of the object
                var stringVersion = _objectToStringProvider.ConvertToString(looseConfigurationObject);

                // Add the version to the container
                Items.Add(stringVersion, looseConfigurationObject);

                // Add the object to the id dictionary
                _idToObjectDictionary.Add(id, looseConfigurationObject);
            }

            // Pull constructed objects
            var constructedObjects = configuration.ConstructedObjects;

            // Initialize all of them
            foreach (var constructedObject in constructedObjects)
            {
                InitializeConstructedObject(constructedObject);
            }
        }

        private static void NullIds(Configuration configuration)
        {
            // Create enumerable for all objects
            var objects = configuration.LooseObjects
                    .Cast<ConfigurationObject>()
                    .Union(configuration.ConstructedObjects);

            // Null id to all of them
            foreach (var configurationObject in objects)
            {
                if (configurationObject == null)
                    throw new ArgumentException("Null inside objects");

                configurationObject.Id = null;
            }

            void NullIdsOfIteriorObjects(ConstructionArgument argument)
            {
                if (argument is ObjectConstructionArgument objectArgument)
                {
                    var passedObject = objectArgument.PassedObject;

                    passedObject.Id = null;

                    if (passedObject is LooseConfigurationObject)
                        return;

                    var args = ((ConstructedConfigurationObject) passedObject).PassedArguments;

                    foreach (var constructionArgument in args)
                    {
                        NullIdsOfIteriorObjects(constructionArgument);
                    }

                    return;
                }

                var setArgument = (SetConstructionArgument) argument ?? throw new GeneratorException("Unhandled case");

                foreach (var passedArgument in setArgument.PassedArguments)
                {
                    NullIdsOfIteriorObjects(passedArgument);
                }
            }

            var allArguments = configuration.ConstructedObjects.SelectMany(obj => obj.PassedArguments);

            foreach (var argument in allArguments)
            {
                NullIdsOfIteriorObjects(argument);
            }
        }

        private void InitializeConstructedObject(ConstructedConfigurationObject constructedObject)
        {
            void ValidateInterior(ConstructionArgument argument)
            {
                if (argument is ObjectConstructionArgument objArgument)
                {
                    var passedObject = objArgument.PassedObject;

                    if (!passedObject.Id.HasValue)
                        ThrowUnconstructableObjectException();

                    if (passedObject is LooseConfigurationObject)
                        return;

                    var args = ((ConstructedConfigurationObject) passedObject).PassedArguments;

                    foreach (var interiorArgument in args)
                    {
                        ValidateInterior(interiorArgument);
                    }

                    return;
                }

                var setArgument = (SetConstructionArgument) argument ?? throw new GeneratorException("Unhandled case");

                foreach (var passedArgument in setArgument.PassedArguments)
                {
                    ValidateInterior(passedArgument);
                }
            }

            // If the object has value, then we have duplicate objects, since it's impossible
            // to be initialzed yet
            if (constructedObject.Id.HasValue)
                throw new InitializationException("Duplicate objects");

            foreach (var passedArgument in constructedObject.PassedArguments)
            {
                ValidateInterior(passedArgument);
            }

            // Now we can freely add the object to the container
            var result = Add(constructedObject);

            // If we already have this object in the container, we have a problem
            if (result != constructedObject)
                throw new InitializationException("Duplicate objects");
        }

        private static void ThrowUnconstructableObjectException()
        {
            throw new InitializationException("Configuration is not constructable");
        }

        #endregion

        #region IEnumerable overriden methods

        public override IEnumerator<ConfigurationObject> GetEnumerator()
        {
            if (!_initialized)
                throw new GeneratorException("Not initialized");

            return base.GetEnumerator();
        }

        #endregion
    }
}