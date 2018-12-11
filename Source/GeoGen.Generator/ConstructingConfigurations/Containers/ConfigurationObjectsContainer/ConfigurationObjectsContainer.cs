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
        /// Adds a given configuration object to the container. The object must not be identified 
        /// while it's being added. If an equal version of the object is present in the container, 
        /// the object won't be added and the <paramref name="equalObject"/> will be set to that
        /// equal object. Otherwise the <paramref name="equalObject"/> will be set to null.
        /// </summary>
        /// <param name="configurationObject">The configuration object to be added.</param>
        /// <param name="equalObject">Either the equal version of the passed object from the container (if there's any), or null.</param>
        public void Add(ConfigurationObject configurationObject, out ConfigurationObject equalObject)
        {
            // Convert the object to string
            var stringRepresentation = _converter.ConvertToString(configurationObject);

            // If we have it cached, we can return it directly 
            if (Items.ContainsKey(stringRepresentation))
            {
                // Set the equal object
                equalObject = Items[stringRepresentation];

                // Terminate
                return;
            }

            // Assign the id to the object
            configurationObject.Id = Items.Count + 1;

            // Add the object to the container 
            Items.Add(stringRepresentation, configurationObject);

            // Cache the gotten string version (it couldn't have happened during
            // the conversion because we hadn't had the id yet)
            _converter.CacheObject(configurationObject.Id, stringRepresentation);

            // Set the equal object
            equalObject = null;            
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
            // First we initialize the loose objects. 
            foreach (var looseObject in configuration.LooseObjects)
            {
                // Get the string version of the object
                var stringVersion = Converter.ConvertToString(looseObject);

                // Add the object to the container
                Items.Add(stringVersion, looseObject);
            }

            // Now initialize the constructed objects
            foreach (var constructedObject in configuration.ConstructedObjects)
            {
                // Call the other method to do the job
                InitializeConstructedObject(constructedObject);
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

            // Otherwise we can use the local function to validate its passed arguments
            foreach (var passedArgument in constructedObject.PassedArguments)
            {
                ValidateInterior(passedArgument);
            }

            // If there are fine, we can freely add the object to the container
            Add(constructedObject, out var equalObject);

            // If we already have this object in the container, we have a problem
            if (equalObject != null)
                throw new InitializationException("The initial configuration contains duplicate objects");

            // Otherwise everything is correct
        }

        #endregion
    }
}