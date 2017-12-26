using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationObjectsContainer"/>
    /// using <see cref="StringBasedContainer{T}"/>, where T is <see cref="ConfigurationObject"/>.
    /// This string based container uses <see cref="DefaultFullObjectToStringConverter"/>.
    /// This sealed class is not thread-safe.
    /// </summary>
    internal sealed class ConfigurationObjectsContainer : StringBasedContainer<ConfigurationObject>, IConfigurationObjectsContainer
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping ids of objects to actual objects.
        /// </summary>
        private readonly Dictionary<int, ConfigurationObject> _idToObjectDictionary;

        /// <summary>
        /// The default full object to string provider.
        /// </summary>
        private readonly IDefaultFullObjectToStringConverter _converter;

        #endregion

        #region IConfigurationObjectsContainer properties

        /// <summary>
        /// Gets the object with a given id. Throws an <see cref="KeyNotFoundException"/>, 
        /// if not present.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The configuration object with the given id.</returns>
        public ConfigurationObject this[int id] => _idToObjectDictionary[id];

        /// <summary>
        /// Gets the loose objects that are present in the container. 
        /// </summary>
        public IEnumerable<LooseConfigurationObject> LooseObjects { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a configuration objects container that is string-based 
        /// and uses a given default object to string provider and contains the objects
        /// from a given initial configuration.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        /// <param name="converter">The object to string provider.</param>
        public ConfigurationObjectsContainer(Configuration initialConfiguration, IDefaultFullObjectToStringConverter converter)
                : base(converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            _idToObjectDictionary = new Dictionary<int, ConfigurationObject>();
            Initialize(initialConfiguration);
        }

        #endregion

        #region IConfigurationObjectsContainer methods

        /// <summary>
        /// Adds a given constructed configuration object to the container. 
        /// The current id of the object will be ignored. If an equal version 
        /// of the object  is present in the container, it will return instance of 
        /// this internal object. Otherwise it will return this object with set id.
        /// </summary>
        /// <param name="constructedObject">The constructed configuration object.</param>
        /// <returns>The identified version of the constructed configuration object.</returns>
        public ConstructedConfigurationObject Add(ConstructedConfigurationObject constructedObject)
        {
            if (constructedObject == null)
                throw new ArgumentNullException(nameof(constructedObject));

            // Null the id
            constructedObject.Id = null;

            // Convert the object to string
            var stringRepresentation = Converter.ConvertToString(constructedObject);

            // If we have it cached, we can return it immediately 
            if (Items.ContainsKey(stringRepresentation))
            {
                return (ConstructedConfigurationObject) Items[stringRepresentation];
            }

            // Otherwise we have a new object. Assign an id to it
            var newId = Items.Count + 1;
            constructedObject.Id = newId;

            // Add it to the container 
            Items.Add(stringRepresentation, constructedObject);

            // Add it to the id to object dictionary 
            _idToObjectDictionary.Add(newId, constructedObject);

            // Cache the gotten string version
            _converter.CacheObject(newId, stringRepresentation);

            // Return the object
            return constructedObject;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Initializes the container with a given configuration. The configuration must not
        /// contain duplicate objects and must be constructible, otherwise an
        /// <see cref="InitializationException"/> will be thrown.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        private void Initialize(Configuration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Null ids of all objects and arguments in the configuration
            NullIds(configuration);

            // Clear the container's content
            Items.Clear();

            // Clear the id dictionary
            _idToObjectDictionary.Clear();

            // Initialize counter
            var counter = 1;

            // Initialize loose objects list
            var looseObjects = new List<LooseConfigurationObject>();

            // Add loose objects to the container
            foreach (var looseConfigurationObject in configuration.LooseObjects)
            {
                // Get new id and increase the counter
                var id = counter++;

                // Assign the id
                looseConfigurationObject.Id = id;

                // Get the string version of the object
                var stringVersion = Converter.ConvertToString(looseConfigurationObject);

                // Add the version to the container
                Items.Add(stringVersion, looseConfigurationObject);

                // Add the object to the id dictionary
                _idToObjectDictionary.Add(id, looseConfigurationObject);

                // Add loose objects to the loose objects list
                looseObjects.Add(looseConfigurationObject);
            }

            // Assign the loose objects property
            LooseObjects = looseObjects;

            // Pull constructed objects
            var constructedObjects = configuration.ConstructedObjects;

            // Initialize all of them
            foreach (var constructedObject in constructedObjects)
            {
                InitializeConstructedObject(constructedObject);
            }
        }

        /// <summary>
        /// Null all the ids of the configurations objects present in a
        /// given configuration directly, or within arguments of constructed
        /// objects of the configuration.
        /// </summary>
        private static void NullIds(Configuration configuration)
        {
            // Create enumerable for all objects
            var objects = configuration.LooseObjects
                    .Cast<ConfigurationObject>()
                    .Concat(configuration.ConstructedObjects);

            // Null id to all of them
            foreach (var configurationObject in objects)
            {
                if (configurationObject == null)
                    throw new ArgumentException("Null inside objects");

                configurationObject.Id = null;
            }

            // Local recursive function to null the interior of a given argument
            void NullIdsOfIteriorObjects(ConstructionArgument argument)
            {
                // If we have an object argument
                if (argument is ObjectConstructionArgument objectArgument)
                {
                    // We can pull the object
                    var passedObject = objectArgument.PassedObject;

                    // Null it's id
                    passedObject.Id = null;

                    // If the object is loose, we can't do anything else
                    if (passedObject is LooseConfigurationObject)
                        return;

                    // Otherwise the object is constructed and therefore we can pull passed arguments
                    var args = ((ConstructedConfigurationObject) passedObject).PassedArguments;

                    // And recursively null all of them
                    foreach (var constructionArgument in args)
                    {
                        NullIdsOfIteriorObjects(constructionArgument);
                    }

                    // And terminate
                    return;
                }

                // Otherwise we have a set argument
                var setArgument = (SetConstructionArgument) argument;

                // And we simply recursively null ids of all passed arguments
                foreach (var passedArgument in setArgument.PassedArguments)
                {
                    NullIdsOfIteriorObjects(passedArgument);
                }
            }

            // First we merge all arguments from all configuration objects
            var allArguments = configuration.ConstructedObjects.SelectMany(obj => obj.PassedArguments);

            // And call the local nulling function for each of them
            foreach (var argument in allArguments)
            {
                NullIdsOfIteriorObjects(argument);
            }
        }

        /// <summary>
        /// Initialized a given constructed configuration object. This method
        /// supposes that all loose and configuration objects have been correctly
        /// initialized.
        /// </summary>
        /// <param name="constructedObject">The constructed configuration object.</param>
        private void InitializeConstructedObject(ConstructedConfigurationObject constructedObject)
        {
            // Local function to validate if the interior of a given argument is correct
            void ValidateInterior(ConstructionArgument argument)
            {
                // If the argument is an object argument
                if (argument is ObjectConstructionArgument objArgument)
                {
                    // Then we call pull the passed object
                    var passedObject = objArgument.PassedObject;

                    // It must be constructible, i.e. it must be already present in the
                    // container, i.e. its id must be set
                    if (!passedObject.Id.HasValue)
                        throw new InitializationException("Configuration is not constructible");

                    // If the object is loose, we're fine
                    if (passedObject is LooseConfigurationObject)
                        return;

                    // Otherwise it's a constructed object and we can pull the passed arguments.
                    var args = ((ConstructedConfigurationObject) passedObject).PassedArguments;

                    // And recursively validate each of them.
                    foreach (var interiorArgument in args)
                    {
                        ValidateInterior(interiorArgument);
                    }

                    // And terminate
                    return;
                }

                // Otherwise we have a set argument
                var setArgument = (SetConstructionArgument) argument;

                // And we simply recursively validate each of passed arguments
                foreach (var passedArgument in setArgument.PassedArguments)
                {
                    ValidateInterior(passedArgument);
                }
            }

            // If the object has id, then we have duplicate objects, since the object
            // is not supposed be initialized yet
            if (constructedObject.Id.HasValue)
                throw new InitializationException("Duplicate objects");

            // Otherwise we can use the local function to validate its passed arguments
            foreach (var passedArgument in constructedObject.PassedArguments)
            {
                ValidateInterior(passedArgument);
            }

            // If there are fine, we can freely add the object to the container
            var result = Add(constructedObject);

            // If we already have this object in the container, we have a problem
            if (result != constructedObject)
                throw new InitializationException("Duplicate objects");

            // Otherwise everything is correct
        }

        #endregion
    }
}