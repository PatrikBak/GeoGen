using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Castle.Core.Internal;
using GeoGen.Analyzer;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationsValidator"/>.
    /// This class rules out configurations that contain duplicate or 
    /// unconstructible objects. It uses an <see cref="IGeometryRegistrar"/>
    /// to determine the geometrical state of these objects.
    /// </summary>
    internal class ConfigurationsResolver : IConfigurationsValidator
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
        private readonly HashSet<int> _unconstructibleIds = new HashSet<int>();

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
        }

        #endregion

        #region IConfigurationResolver implementation

        /// <summary>
        /// Validates the configuration, which means checking if it contains duplicate or inconstructible objects.
        /// </summary>
        /// <param name="configuration">The configuration to be validated.</param>
        /// <returns>true, if the configuration is valid, false otherwise</returns>
        public bool Validate(Configuration configuration)
        {
            // TODO: Optimize for newly constructed configurations

            // First let's take object ids
            var objectIds = configuration.ObjectsIds();

            // Check if there is no duplicate, i.e. the number of ids is equal to the number of objects
            if (configuration.NumberOfObjects != objectIds.Count)
                return false;

            // Check if there is no id that has been marked as forbidden
            if (objectIds.Any(_unconstructibleIds.Contains))
                return false;


            Test.a1++;
            var sw = new Stopwatch();
            sw.Start();

            // At this point, we're sure that the new configuration is formally correct.
            // Now we find out if it's geometrically correct. Let's call the registrar
            var registrationResult = _registrar.Register(configuration);

            sw.Stop();
            Test.t1 += sw.ElapsedMilliseconds;

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
                    _unconstructibleIds.Add(unconstructibleObject.Id);
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

        #endregion
    }
}