using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using GeoGen.Analyzer;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationsResolver"/>.
    /// This class rules out configurations that contain duplicate or 
    /// unconstructible objects. It uses an <see cref="IGeometryRegistrar"/>
    /// to determine the geometrical state of these objects.
    /// </summary>
    internal class ConfigurationsResolver : IConfigurationsResolver
    {
        #region Private fields

        /// <summary>
        /// The geometry registrar used to register new configurations.
        /// </summary>
        private readonly IGeometryRegistrar _registrar;

        /// <summary>
        /// The container used to identify new geometrical objects.
        /// </summary>
        private readonly IConfigurationObjectsContainer _container;

        /// <summary>
        /// The set containing ids of unconstructible objects. Any configuration containing
        /// such objects should be resolved as incorrect.
        /// </summary>
        private readonly HashSet<int> _unconstructibleIds;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="registrar">The registrar for registering new configurations.</param>
        /// <param name="container">The container for identifying new objects.</param>
        public ConfigurationsResolver(IGeometryRegistrar registrar, IConfigurationObjectsContainer container)
        {
            _registrar = registrar ?? throw new ArgumentNullException(nameof(registrar));
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _unconstructibleIds = new HashSet<int>();
        }

        #endregion

        #region IConfigurationResolver implementation

        /// <summary>
        /// Resolves a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>true, if the configuration was resolved as correct, false otherwise</returns>
        public bool ResolveNewConfiguration(Configuration configuration)
        {
            // First check if the new objects of the configuration are correct
            if (!AreNewObjectsCorrect(configuration))
                // If they're not, return failure
                return false;

            // At this point, we're sure that the new configuration is formally correct.
            // Now we find out if it's geometrically correct. Let's call the registrar
            var registrationResult = _registrar.Register(configuration);

            // Find out if there is an uncontructible object
            var anyUnconstructibleObject = !registrationResult.UnconstructibleObjects.IsNullOrEmpty();

            // Find out if there are any duplicates
            var anyDuplicates = !registrationResult.Duplicates.IsNullOrEmpty();

            // Find out if the configuration is correct, i.e. constructible and without duplicates
            var isCorrect = !anyDuplicates && !anyUnconstructibleObject;

            // If the configuration is correct, we're fine
            if (isCorrect)
                return true;

            // Otherwise it's not correct...There are either unconstructible objects, or duplicates

            // If there are unconstructible objects..
            if (anyUnconstructibleObject)
            {
                // Then we want to remember their ids
                foreach (var unconstructibleObject in registrationResult.UnconstructibleObjects)
                {
                    _unconstructibleIds.Add(unconstructibleObject.Id.Value);
                }
            }

            // If there are duplicates...
            if (anyDuplicates)
            {
                // TODO: Do something about it
            }

            // Return failure
            return false;
        }

        /// <summary>
        /// Checks if the new objects of the configuration are correct. They'll get 
        /// identified on go. 
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>true, if the new objects are correct; false otheriwse.</returns>
        private bool AreNewObjectsCorrect(Configuration configuration)
        {
            // Pull ids of initial constructed objects and cast them to set. 
            // The ids of new objects shouldn't be present in the set.
            var initialIds = configuration
                    // Pull constructed objects                            
                    .ConstructedObjects
                    // Initial objects are identitfied                            
                    .Where(obj => obj.Id != null)
                    // Cast them to their ids
                    .Select(obj => obj.Id.Value)
                    // And then wrap to a sets
                    .ToSet();

            // Determine if there are no duplicates and all new objects are not forbidden
            foreach (var constructedObject in configuration.LastAddedObjects)
            {
                // Add the object to the container
                var containerResult = _container.Add(constructedObject);

                // Pull the id
                var id = containerResult.Id ?? throw new GeneratorException("The configuration objects container was supposed to set the id.");

                // Set the id of the object
                constructedObject.Id = id;

                // If the object is currently in the configuration
                if (initialIds.Contains(id))
                {
                    // Return failure
                    return false;
                }

                // Find out if it's forbidden or unconstructible
                var isIncorrect = _unconstructibleIds.Contains(id);

                // If the object with this id is incorrect
                if (isIncorrect)
                {
                    // We want to resolve it as incorrect
                    return false;
                }
            }

            // If we got here, then all objects are fine
            return true;
        }

        /// <summary>
        /// Resolves a given initial configuration. If the configuration
        /// is resolved as incorrect, then an <see cref="InitializationException"/>
        /// will be thrown.
        /// </summary>
        /// <param name="configuration">The initial configuration.</param>
        public void ResolveInitialConfiguration(Configuration configuration)
        {
            // Call the service to register the configuration
            var registrationResult = _registrar.Register(configuration);

            // Find out if there is an uncontructible object
            var anyUnconstructibleObject = !registrationResult.UnconstructibleObjects.IsNullOrEmpty();

            // Handle possible unconstructible objects
            if (anyUnconstructibleObject)
                throw new InitializationException("Initial configuration contains unconstructible objects.");

            // Find out if there are any duplicates
            var anyDuplicates = !registrationResult.Duplicates.IsNullOrEmpty();

            // Handle possible duplicates
            if (anyDuplicates)
                throw new InitializationException("Initial configuration contains duplicates.");

            // Otherwise we're fine
        }

        #endregion
    }
}