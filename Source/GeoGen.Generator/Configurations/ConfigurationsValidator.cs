using GeoGen.Analyzer;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationsValidator"/>.
    /// This class rules out configurations that contain duplicate or 
    /// unconstructible objects. It uses an <see cref="IGeometryRegistrar"/>
    /// to determine the geometrical state of these objects.
    /// </summary>
    public class ConfigurationsValidator : IConfigurationsValidator
    {
        #region Dependencies

        /// <summary>
        /// The registrar used to draw new configurations.
        /// </summary>
        private readonly IGeometryRegistrar _registrar;

        /// <summary>
        /// The container that recognizes equal configurations.
        /// </summary>
        private readonly IContainer<GeneratedConfiguration> _container;

        #endregion

        #region Private fields

        /// <summary>
        /// The set containing ids of unconstructible objects. Any configuration containing
        /// such objects should be resolved as incorrect.
        /// </summary>
        private readonly HashSet<int> _unconstructibleIds = new HashSet<int>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationsValidator"/> class.
        /// </summary>
        /// <param name="registrar">The registrar used to draw new configurations.</param>
        /// <param name="container">The container that recognizes equal configurations.</param>
        public ConfigurationsValidator(IGeometryRegistrar registrar, IContainer<GeneratedConfiguration> container)
        {
            _registrar = registrar ?? throw new ArgumentNullException(nameof(registrar));
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        #endregion

        #region IConfigurationsValidator implementation

        /// <summary>
        /// Perform the validation on a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration to be validated.</param>
        /// <returns>true, if the configuration is valid, false otherwise</returns>
        public bool Validate(GeneratedConfiguration configuration)
        {
            #region Object ids based validation

            // Let's take object ids
            var objectIds = configuration.ObjectsIds();

            // Check if there is no duplicate, i.e. the number of ids is equal to the number of objects
            if (configuration.NumberOfObjects != objectIds.Count)
                return false;

            // Check if there is any id that has been marked as unconstructible
            if (objectIds.Any(_unconstructibleIds.Contains))
                return false;

            // We're sure the configuration is formally correct (no duplicates)
            // and that can't be exluded based on previous data (unconstructible objects)

            #endregion

            #region Equal configurations validation

            // Add the configuration to the container
            _container.Add(configuration, out var equalConfiguration);

            // If there already is an equal version of it, this one is invalid
            if (equalConfiguration != null)
                return false;

            // We're sure the configuration is new, i.e. we haven't seen an equal or isomorphic version of it yet

            #endregion

            #region Geometrical validation

            // Call the registrar to get the geometrical info about the configuration
            var registrationResult = _registrar.Register(configuration);

            // Find out if there is an uncontructible object
            var anyUnconstructibleObject = !registrationResult.UnconstructibleObjects.IsNullOrEmpty();

            // Find out if there are any duplicates
            var anyDuplicates = !registrationResult.Duplicates.IsNullOrEmpty();

            // Find out if the configuration is correct, i.e. constructible and without duplicates
            var isCorrect = !anyDuplicates && !anyUnconstructibleObject;

            // If there are unconstructible objects..
            if (anyUnconstructibleObject)
            {
                // Then we want to remember their ids
                registrationResult.UnconstructibleObjects.ForEach(obj => _unconstructibleIds.Add(obj.Id));
            }

            // If there are duplicates...
            if (anyDuplicates)
            {
                // TODO: Do something about it
            }

            // If there are any invalid objects or duplicates, then it's incorrect
            if (anyUnconstructibleObject || anyDuplicates)
                return false;

            #endregion

            // After all validations passed, return that it's valid
            return true;
        }

        #endregion
    }
}