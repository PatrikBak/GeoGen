using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Utilities.StringBasedContainer;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationsConstructing.IdsFixing;
using GeoGen.Generator.ConstructingObjects.Arguments.Containers;

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
        /// Gets the object with a given id. Throws an exception, if not present.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The configuration object with the given id.</returns>
        public ConfigurationObject this[int id] => _idToObjectDictionary[id];

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

        /// <summary>
        /// The argument container.
        /// </summary>
        private readonly IArgumentContainer _container;

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
        public ConfigurationObjectsContainer(DefaultFullObjectToStringProvider provider, IArgumentContainer container)
        {
            ConfigurationObjectToStringProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            _container = container ?? throw new ArgumentNullException(nameof(container));
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
            return ConfigurationObjectToStringProvider.ConvertToString(item);
        }

        #endregion

        #region IConfigurationObjectsContainer methods

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
            ConfigurationObjectToStringProvider.CacheObject(newId, stringRepresentation);

            // Return the object
            return constructedConfigurationObject;
        }

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

            var counter = 1;

            // Add loose objects to the container
            foreach (var looseConfigurationObject in configuration.LooseObjects)
            {
                // Get new id and increase the counter
                var id = counter++;

                // Assign the id
                looseConfigurationObject.Id = id;

                // Get the string version of the object
                var stringVersion = ConfigurationObjectToStringProvider.ConvertToString(looseConfigurationObject);

                // Add the version to the container and to the id dictionary
                Items.Add(stringVersion, looseConfigurationObject);
                _idToObjectDictionary.Add(id, looseConfigurationObject);
            }

            // Pull constructed objects
            var constructedObjects = configuration.ConstructedObjects;

            // Add objects to the container
            foreach (var constructedObject in constructedObjects)
            {
                InitializeConstructedObject(constructedObject, false);
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

            // Local function to enumerate all construction arguments
            IEnumerable<ConstructionArgument> Args(ConstructionArgument argument)
            {
                // If we have just an object argument, we'll yield that
                if (argument is ObjectConstructionArgument objectArgument)
                {
                    // Null the id to object argument on go
                    objectArgument.Id = null;

                    // And yield it
                    yield return argument;

                    // And break the method
                    yield break;
                }

                // Otherwise we must have a set argument
                var setArg = argument as SetConstructionArgument ?? throw new GeneratorException("Unhandled case");

                // We iterate over all passed arguments to it
                foreach (var passedArgument in setArg.PassedArguments)
                {
                    // And then through all recurvisely gotten args from the passed argumet
                    foreach (var arg in Args(passedArgument))
                    {
                        // And yield each
                        yield return arg;
                    }
                }
            }

            // Create enumeration for all args
            var args = configuration.ConstructedObjects
                    .SelectMany(obj => obj.PassedArguments)
                    .SelectMany(Args);

            // Null id to all of them
            foreach (var constructionArgument in args)
            {
                constructionArgument.Id = null;
            }
        }

        private void InitializeConstructedObject(ConstructedConfigurationObject constructedObject, bool passedAsArgument)
        {
            // If the object is passed as argument
            if (passedAsArgument)
            {
                // Then it must be already in the container, so
                // it must have set the id. If it doesn't, we have a problem
                if (!constructedObject.Id.HasValue)
                    UnconstructableException();

                // Otherwise this object has already been initialized so we're fine
                return;
            }

            // If the object has value, then we have duplicate objects, since it's impossible
            // to be initialzed yet
            if (constructedObject.Id.HasValue)
                throw new InitializationException("Duplicate objects");

            // Otherwise the object is new and needs to be initialized. 
            // First we need to initialize its arguments
            foreach (var argument in constructedObject.PassedArguments)
            {
                InitializeArgument(argument);
            }

            // Now we can freely add the object to the container
            var result = Add(constructedObject);

            // If we already have this object in the container, we have a problem
            if (result != constructedObject)
                throw new InitializationException("Duplicate objects");

            // Otherwise we're fine. The id of the object is set.
        }

        private void InitializeArgument(ConstructionArgument argument)
        {
            // Local function for adding the argument to the container
            void AddToContainer()
            {
                // And finally add the argument to the container
                var result = _container.AddArgument(argument);

                // The result may or may not be the same. We identify the argument by id
                argument.Id = result.Id;
            }

            // If the argument has id, then it's been already initialized, so we're fine
            if (argument.Id.HasValue)
                return;

            // If we have an object argument
            if (argument is ObjectConstructionArgument objectArgument)
            {
                // Pull passed object
                var passedObject = objectArgument.PassedObject;

                // If the passed object is loose
                if (passedObject is LooseConfigurationObject)
                {
                    // If the id of the isn't set, the we have a problem
                    if (!passedObject.Id.HasValue)
                        UnconstructableException();

                    // Otherwise we've run into a correct object. 
                    // We can add it
                    AddToContainer();

                    // And terminate
                    return;
                }

                // Otherwise we have constructed object
                var constructedObj = passedObject as ConstructedConfigurationObject ?? throw new GeneratorException("Unhandled case");

                // We let the method initialize it
                InitializeConstructedObject(constructedObj, true);

                // Finally add the argument to the container
                AddToContainer();

                // And terminate
                return;
            }

            // Otherwise we have a set argument
            var setArgument = argument as SetConstructionArgument ?? throw new ArgumentNullException(nameof(argument));

            // We recursively initialize the passed arguments
            foreach (var passedArgument in setArgument.PassedArguments)
            {
                InitializeArgument(passedArgument);
            }

            // Finally add the argument to the container
            AddToContainer();
        }

        private void UnconstructableException() => throw new InitializationException("Configuration is not constructable");

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