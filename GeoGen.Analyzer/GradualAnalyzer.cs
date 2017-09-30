using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IGradualAnalyzer"/>.
    /// This class is thread-safe.
    /// </summary>
    internal class GradualAnalyzer : IGradualAnalyzer
    {
        #region Private fields

        /// <summary>
        /// The geometrical objects container.
        /// </summary>
        private readonly IGeometricalObjectsContainer _container;

        /// <summary>
        /// The theorem finder.
        /// </summary>
        private readonly ITheoremsFinder _finder;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a gradual analyzer that uses a given objects container
        /// to determine duplicate objects and a given theorem finder.
        /// </summary>
        /// <param name="container">The geometrical objects container.</param>
        /// <param name="finder">THe theorems finder.</param>
        public GradualAnalyzer(IGeometricalObjectsContainer container, ITheoremsFinder finder)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _finder = finder ?? throw new ArgumentNullException(nameof(finder));
        }

        #endregion

        #region IGradualAnalyzer methods

        /// <summary>
        /// Analyses a given configuration provided as two lists, or old and new configuration
        /// objects.
        /// </summary>
        /// <param name="oldObjects">The old objects.</param>
        /// <param name="newObjects">The new objects.</param>
        /// <returns>The analyzer output.</returns>
        public AnalyzerOutput Analyze(List<ConfigurationObject> oldObjects, List<ConfigurationObject> newObjects)
        {
            if (oldObjects == null)
                throw new ArgumentNullException(nameof(oldObjects));

            if (newObjects == null)
                throw new ArgumentNullException(nameof(newObjects));

            // Create fields that will be eventually returned in an output object
            var theorems = new List<Theorem>();
            var duplicateObjects = new Dictionary<ConfigurationObject, ConfigurationObject>();
            var canBeConstructed = true;

            // First we add new objects to the container
            foreach (var newObject in newObjects)
            {
                // If we can successfully add a new object to the container
                // Then we can move forward
                if (_container.Add(newObject, out ConfigurationObject duplicate))
                    continue;

                // Otherwise there is a problem with adding a new object. It means
                // that either there is a duplicate object, is the object is not constructible
                // at all

                // If the duplicate object is null, then we must have an object that is 
                // not constructible. 
                if (duplicate == null)
                {
                    canBeConstructed = false;
                    continue;
                }

                // Otherwise we're sure there is a duplicate object. We can update the 
                // dictionary.
                duplicateObjects.Add(newObject, duplicate);

                // The fact that two objects are duplicate might be interesting, so
                // we construct a theorem for it.

                // Construct involved objects
                var involvedObjects = new List<TheoremObject>
                {
                    new SingleTheoremObject(newObject),
                    new SingleTheoremObject(duplicate)
                };

                // Construct theorem itself
                var theorem = new Theorem(TheoremType.SameObjects, involvedObjects);

                // And update theorems dictionary
                theorems.Add(theorem);
            }

            // Now we're done with adding objects to the container.
            // If we can construct all objects and there are no duplicate objects,
            // we'll do the usual theorem finding
            if (!canBeConstructed && duplicateObjects.Empty())
            {
                var newTheorems = _finder.FindTheorems(oldObjects, newObjects);
                theorems.AddRange(newTheorems);
            }
            // Otherwise we won't analyze the configuration and clean the container
            // from new not-considered objects
            else
            {
                _container.Remove(newObjects);
            }

            // And finally we can construct the result
            return new AnalyzerOutput
            {
                Theorems = theorems,
                CanBeFullyConstructed = canBeConstructed,
                DuplicateObjects = duplicateObjects
            };
        }

        #endregion
    }
}