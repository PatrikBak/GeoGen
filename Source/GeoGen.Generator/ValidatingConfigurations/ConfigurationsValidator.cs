using GeoGen.Analyzer;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Generator
{
    /// <summary>
    /// The default implementation of <see cref="IConfigurationsValidator"/>.
    /// This class first rules out configurations that contain one object twice, or 
    /// ones that contain an object that is already resolved as an inconstructible one.
    /// Then it uses the configurations container to find out if this configuration
    /// is new. Only then it actually performs the geometrical validation
    /// via <see cref="IGeometryRegistrar"/>. It then resolves the configuration
    /// as valid only if it doesn't contain geometric duplicates, or an inconstructible
    /// object. These two aspects can be intercepted via <see cref="IEqualObjectsTracer"/>
    /// and <see cref="IInconstructibleObjectsTracer"/>.
    /// </summary>
    public class ConfigurationsValidator : IConfigurationsValidator
    {
        #region Dependencies

        /// <summary>
        /// The registrar that perform the actual geometrical construction of configurations.
        /// </summary>
        private readonly IGeometryRegistrar _registrar;

        /// <summary>
        /// The container of configuration (initially empty) that recognizes equal configurations.
        /// </summary>
        private readonly IContainer<GeneratedConfiguration> _container;

        /// <summary>
        /// The tracer of geometrically equal objects determined by the registrar.
        /// </summary>
        private readonly IEqualObjectsTracer _equalObjectsTracer;

        /// <summary>
        /// The tracer of geometrically inconstructible objects determined by the registrar.
        /// </summary>
        private readonly IInconstructibleObjectsTracer _inconstructibleObjectsTracer;

        #endregion

        #region Private fields

        /// <summary>
        /// The set containing ids of inconstructible objects. Any configuration 
        /// containing such an object should be resolved as incorrect.
        /// </summary>
        private readonly HashSet<int> _inconstructibleObjectsIds = new HashSet<int>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationsValidator"/> class.
        /// </summary>
        /// <param name="registrar">The registrar that perform the actual geometrical construction of configurations..</param>
        /// <param name="container">The container of configuration (initially empty) that recognizes equal configurations.</param>
        public ConfigurationsValidator(IGeometryRegistrar registrar, IContainer<GeneratedConfiguration> container, IEqualObjectsTracer equalObjectsTracer = null, IInconstructibleObjectsTracer inconstructibleObjectsTracer = null)
        {
            _registrar = registrar ?? throw new ArgumentNullException(nameof(registrar));
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _equalObjectsTracer = equalObjectsTracer;
            _inconstructibleObjectsTracer = inconstructibleObjectsTracer;
        }

        #endregion

        #region IConfigurationsValidator implementation

        /// <summary>
        /// Perform the validation of a given configuration. The internal objects of the configuration
        /// must be correctly identified.
        /// </summary>
        /// <param name="configuration">The configuration to be validated.</param>
        /// <returns>true, if the configuration is valid; false otherwise.</returns>
        public bool Validate(GeneratedConfiguration configuration)
        {
            #region Object ids based validation

            // Let's find the object ids
            var objectIds = configuration.ObjectsMap.AllObjects.Select(obj => obj.Id).ToSet();

            // Check if there is no duplicate, i.e. the number of ids is equal to the number of objects
            if (configuration.ObjectsMap.AllObjects.Count != objectIds.Count)
                return false;

            // Check if there is any id that has been marked as inconstructible
            if (objectIds.Any(_inconstructibleObjectsIds.Contains))
                return false;

            // We're sure the configuration is formally correct (no duplicates)
            // and that can't be excluded based on the previous data (inconstructible objects)

            #endregion

            #region Equal configurations validation

            // Add the configuration to the container
            _container.Add(configuration, out var equalConfiguration);

            // If there already is an equal version of it, this one is invalid
            if (equalConfiguration != null)
                return false;

            // We're sure the configuration is new, i.e. we haven't seen an equal version of it yet

            #endregion

            #region Geometrical validation

            // Call the registrar to get the geometrical info about the configuration
            var registrationResult = _registrar.Register(configuration);

            // Find out if there is an inconstructible object
            var anyUnconstructibleObject = !registrationResult.UnconstructibleObjects.IsNullOrEmpty();

            // Find out if there are any duplicates
            var anyDuplicates = !registrationResult.Duplicates.IsNullOrEmpty();

            // Find out if the configuration is correct, i.e. constructible and without duplicates
            var isCorrect = !anyDuplicates && !anyUnconstructibleObject;

            // If there are inconstructible objects..
            if (anyUnconstructibleObject)
            {
                // Then we want to remember their ids
                registrationResult.UnconstructibleObjects.ForEach(obj => _inconstructibleObjectsIds.Add(obj.Id));

                // And trace them
                registrationResult.UnconstructibleObjects.ForEach(obj => _inconstructibleObjectsTracer?.TraceInconstructibleObject(obj));
            }

            // If there are duplicates...
            if (anyDuplicates)
            {
                // We want to trace them
                registrationResult.Duplicates.ForEach(pair => _equalObjectsTracer?.TraceEqualObjects(pair.olderObject, pair.newerObject));
            }

            // If there are any invalid objects or duplicates, then it's incorrect
            if (anyUnconstructibleObject || anyDuplicates)
                return false;

            #endregion

            // After all the validations passed, return that it's valid
            return true;
        }

        #endregion
    }
}