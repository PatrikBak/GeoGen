using System;
using System.Linq;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationObjectsContainer"/>. It's
    /// implemented as a <see cref="StringBasedContainer{T}"/> that uses a given
    /// <see cref="IDefaultFullObjectToStringConverter"/>. This is the places where
    /// configuration objects gets their ids. Therefore, the full conversion is the 
    /// only way to determine whether two configuration objects are equal or not. Since 
    /// this id is not known during the conversion, we need to manually cache the new string
    /// versions of new objects after the conversion. 
    /// </summary>
    internal class ConfigurationObjectsContainer : StringBasedContainer<ConfigurationObject>, IConfigurationObjectsContainer
    {
        #region Private fields

        /// <summary>
        /// The converter used by the string-based container and by this class
        /// for manual caching.
        /// </summary>
        private readonly IDefaultFullObjectToStringConverter _converter;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a configuration objects container that internally converts
        /// elements to string using a given default full object to string converter.
        /// This container must contain the initial configuration. The ids of its 
        /// internal objects will be ignored. However, the configuration must be 
        /// formally correct, otherwise an <see cref="InitializationException"/> will
        /// be thrown.
        /// </summary>
        /// <param name="initialConfiguration">The initial configuration.</param>
        /// <param name="converter">The converter.</param>
        public ConfigurationObjectsContainer(Configuration initialConfiguration, IDefaultFullObjectToStringConverter converter)
                : base(converter)
        {
            _converter = converter ?? throw new ArgumentNullException(nameof(converter));
            Initialize(initialConfiguration);
        }

        #endregion

        #region IConfigurationObjectsContainer methods

        /// <summary>
        /// Adds a given constructed configuration object to the container. 
        /// The current id of the object will be ignored. If an equal version 
        /// of the object is present in the container, it will return the instance of 
        /// this internal object. Otherwise it will return this object with set id.
        /// </summary>
        /// <param name="constructedObject">The constructed configuration object.</param>
        /// <returns>The equal identified version of the constructed configuration object.</returns>
        public ConstructedConfigurationObject Add(ConstructedConfigurationObject constructedObject)
        {
            // Null the id
            constructedObject.Id = null;

            // Convert the object to string
            var stringRepresentation = _converter.ConvertToString(constructedObject);

            // If we have it cached, we can return it directly 
            if (Items.ContainsKey(stringRepresentation))
            {
                return (ConstructedConfigurationObject) Items[stringRepresentation];
            }

            // Otherwise we have a new object. Prepare the new id.
            var newId = Items.Count + 1;

            // Assign the id to the object
            constructedObject.Id = newId;

            // Add the object to the container 
            Items.Add(stringRepresentation, constructedObject);

            // Cache the gotten string version (it couldn't have happened during
            // the conversion because we hadn't had the yet)
            _converter.CacheObject(newId, stringRepresentation);

            // Return the object
            return constructedObject;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the container with a given configuration. The configuration must not
        /// contain duplicate objects and must be constructible, otherwise an
        /// <see cref="InitializationException"/> will be thrown.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        private void Initialize(Configuration configuration)
        {
            // Null ids of all objects and arguments in the configuration
            NullIds(configuration);

            // First we initialize the loose objects. 
            foreach (var looseObject in configuration.LooseObjects)
            {
                // Get the string version of the object
                var stringVersion = Converter.ConvertToString(looseObject);

                // Add the object to the container
                Items.Add(stringVersion, looseObject);
            }

            // Now we initial the constructed objects
            var constructedObjects = configuration.ConstructedObjects;

            // Initialize all of them
            foreach (var constructedObject in constructedObjects)
            {
                // Call the other method to do the job
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
            // Local function that nulls an object, if it's not a loose object
            void Null(ConfigurationObject configurationObject)
            {
                if(configurationObject is LooseConfigurationObject)
                    return;

                configurationObject.Id = null;
            }

            // Null id to all of them
            foreach (var configurationObject in configuration.ConstructedObjects)
            {
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
                    Null(passedObject);

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
                    if (passedObject.Id == null)
                        throw new InitializationException("The initial configuration is not logically constructible");

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
            if (constructedObject.Id != null)
                throw new InitializationException("The initial configuration contains duplicate objects");

            // Otherwise we can use the local function to validate its passed arguments
            foreach (var passedArgument in constructedObject.PassedArguments)
            {
                ValidateInterior(passedArgument);
            }

            // If there are fine, we can freely add the object to the container
            var result = Add(constructedObject);

            // If we already have this object in the container, we have a problem
            if (result != constructedObject)
                throw new InitializationException("The initial configuration contains duplicate objects");

            // Otherwise everything is correct
        }

        #endregion
    }
}