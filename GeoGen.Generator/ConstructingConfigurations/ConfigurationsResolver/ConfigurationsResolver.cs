using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer;
using GeoGen.Core.Configurations;
using GeoGen.Core.Generator;
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
        /// The geometry registrar used to register new objects.
        /// </summary>
        private readonly IGeometryRegistrar _registrar;

        /// <summary>
        /// The container used to identify new geometrical objects.
        /// </summary>
        private readonly IConfigurationObjectsContainer _container;

        /// <summary>
        /// The set containing ids of forbidden objects. These objects are either
        /// unconstructible, or there already is a geometrically equal version 
        /// of them. Such objects should not appear in a configuration that is
        /// further analyzed.
        /// </summary>
        private readonly HashSet<int> _forbiddenObjectsIds;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="registrar">The registrar for registering new objects to the real geometrical world.</param>
        /// <param name="container">The container for identifying new objects.</param>
        public ConfigurationsResolver(IGeometryRegistrar registrar, IConfigurationObjectsContainer container)
        {
            _registrar = registrar ?? throw new ArgumentNullException(nameof(registrar));
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _forbiddenObjectsIds = new HashSet<int>();
        }

        #endregion

        #region IConfigurationResolver implementation

        /// <summary>
        /// Resolves a given constructor output.
        /// </summary>
        /// <param name="output">The constructor output.</param>
        /// <returns>true, if the output was resolved as correct, false otherwise</returns>
        public bool ResolveNewOutput(ConstructorOutput output)
        {
            // Let the helper method construct new objects and find out if some 
            // of them aren't forbidden or duplicate
            var newObjects = ConstructNewObjects(output, out var correctObjects);

            // If the new objects aren't correct, return failure
            if (!correctObjects)
                return false;

            // Otherwise re-assign the output (we need to do this because the interior
            // objects might have changed, because the original ones had no ids whereas
            // these have been identified by the container)
            output.ConstructedObjects = newObjects;

            // And return success
            return true;
        }

        /// <summary>
        /// Resolves a given initial configuration. If the configuration
        /// is resolved as incorrect, then an <see cref="InitializationException"/>
        /// will be thrown.
        /// </summary>
        /// <param name="configuration">The initial configuration.</param>
        public void ResolveInitialConfiguration(ConfigurationWrapper configuration)
        {
            // Iterate over all grouped constructed objects
            foreach (var constructedObjects in configuration.WrappedConfiguration.GroupConstructedObjects())
            {
                // Let the helper method do the registration
                var registrationResult = Register(constructedObjects);

                // Switch over the result. Only Ok result is acceptable for 
                // the initial configuration.
                switch (registrationResult)
                {
                    case RegistrationResult.Unconstructible:
                        throw new InitializationException("Initial configuration contains unconstructible objects.");
                    case RegistrationResult.Duplicates:
                        throw new InitializationException("Initial configuration contains duplicates.");
                }
            }
        }

        /// <summary>
        /// Constructs new objects from a given output and determines if they
        /// are correct (i.e. there are no duplicates or forbidden objects).
        /// </summary>
        /// <param name="output">The constructor output.</param>
        /// <param name="correctObjects">The correct objects flag.</param>
        /// <returns>The list of new constructed objects.</returns>
        private List<ConstructedConfigurationObject> ConstructNewObjects(ConstructorOutput output, out bool correctObjects)
        {
            // Initialize result
            var result = new List<ConstructedConfigurationObject>();

            // Pull ids of initial constructed objects and cast them to set. 
            // The ids of new objects shouldn't be present in the set.
            var initialIds = output
                    .OriginalConfiguration
                    .WrappedConfiguration
                    .ConstructedObjects
                    .Select(obj => obj.Id ?? throw GeneratorException.ObjectIdNotSet())
                    .ToSet();

            // Determine if there are no duplicates and all objects are not forbidden
            foreach (var constructedObject in output.ConstructedObjects)
            {
                // Add the object to the container
                var containerResult = _container.Add(constructedObject);

                // Pull the id
                var id = containerResult.Id ?? throw GeneratorException.ObjectIdNotSet();

                // If the object is currently in the configuration
                if (initialIds.Contains(id))
                {
                    // Set the flag indicating that there is an incorrect object
                    correctObjects = false;

                    // Terminate
                    return null;
                }

                // Find out if it's forbidden or unconstructible
                var isIncorrect = _forbiddenObjectsIds.Contains(id);

                // If the object with this id is incorrect
                if (isIncorrect)
                {
                    // Set the flag indicating that there is an incorrect object
                    correctObjects = false;

                    // Terminate
                    return null;
                }

                // Otherwise we add the result to the result list
                result.Add(containerResult);
            }

            // At this point, we're sure that the new configuration is formally correct.
            // Now we find out if it's geometrically correct. Let the helper method do the job.
            var isCorrectAfterRegistration = Register(result) == RegistrationResult.Ok;

            // If the object is incorrect
            if (!isCorrectAfterRegistration)
            {
                // Set the flag indicating that there is an incorrect object
                correctObjects = false;

                // Terminate
                return null;
            }

            // If we got here, then we have only correct objects
            correctObjects = true;

            // Therefore we can return the objects
            return result;
        }

        /// <summary>
        /// Registers given objects from a single constructor output to the 
        /// <see cref="IGeometryRegistrar"/> and handles it's result.
        /// </summary>
        /// <param name="constructedObjects">The constructed objects.</param>
        /// <returns>The registration result.</returns>
        private RegistrationResult Register(List<ConstructedConfigurationObject> constructedObjects)
        {
            // Call the registrar
            var result = _registrar.Register(constructedObjects);

            // Find out if objects are correct
            var correct = result == RegistrationResult.Ok;

            // If the objects are correctly drawable, return immediately
            if (correct)
                return result;

            // Otherwise pull ids
            var ids = constructedObjects.Select(o => o.Id ?? throw GeneratorException.ObjectIdNotSet());

            // Update the forbidden ids set
            foreach (var id in ids)
            {
                _forbiddenObjectsIds.Add(id);
            }

            // And return the result
            return result;
        }

        #endregion
    }
}