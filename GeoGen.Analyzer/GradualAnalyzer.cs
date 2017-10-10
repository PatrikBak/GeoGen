using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Analyzer.Objects;
using GeoGen.Analyzer.Theorems;
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
        /// The geometry holder.
        /// </summary>
        private readonly IGeometryHolder _holder;

        private readonly ITheoremVerifier[] _verifiers;

        private readonly ITheoremsContainer _container;

        #endregion

        #region IGradualAnalyzer methods

        /// <summary>
        /// Analyses a given configuration provided as two lists, or old and new configuration
        /// objects.
        /// </summary>
        /// <param name="oldObjects">The old objects.</param>
        /// <param name="newObjects">The new objects.</param>
        /// <returns>The analyzer output.</returns>
        public AnalyzerOutput Analyze(List<ConfigurationObject> oldObjects, List<ConstructedConfigurationObject> newObjects)
        {
            if (oldObjects == null)
                throw new ArgumentNullException(nameof(oldObjects));

            if (newObjects == null)
                throw new ArgumentNullException(nameof(newObjects));

            var result = _holder.Register(newObjects);

            var duplicateObjects = result.DuplicateObjects;
            var canBeConstructed = result.CanBeConstructed;
            var theorems = new List<Theorem>();

            foreach (var pair in duplicateObjects)
            {
                var newObject = pair.Key;
                var duplicate = pair.Value;

                duplicateObjects.Add(newObject, duplicate);

                // Construct involved objects
                var involvedObjects = new List<TheoremObject>
                {
                    new TheoremObject(newObject),
                    new TheoremObject(duplicate)
                };

                // Construct theorem itself
                var theorem = new Theorem(TheoremType.SameObjects, involvedObjects);

                // TODO: I'm not sure if this is needed when the handler is properly implemented
                if (_container.Add(theorem))
                    theorems.Add(theorem);
            }

            // Now we're done with adding objects to the container.
            // If we can construct all objects and there are no duplicate objects,
            // we'll do the usual theorem finding
            if (canBeConstructed && duplicateObjects.Empty())
            {
                var oldObjectsMap = new ConfigurationObjectsMap(oldObjects);
                var newObjectsMap = new ConfigurationObjectsMap(newObjects);

                theorems.AddRange(FindTheoems(oldObjectsMap, newObjectsMap));
            }

            // And finally we can construct the result
            return new AnalyzerOutput
            {
                Theorems = theorems,
                CanBeFullyConstructed = canBeConstructed,
                DuplicateObjects = duplicateObjects
            };
        }

        private IEnumerable<Theorem> FindTheoems(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects)
        {
            var result = new List<Theorem>();

            foreach (var theoremVerifier in _verifiers)
            {
                var output = theoremVerifier.GetOutput(oldObjects, newObjects);

                foreach (var verifierOutput in output)
                {
                    if (_holder.All(container => verifierOutput.VerifierFunction(container)))
                    {
                        result.Add(verifierOutput.Theorem());
                    }
                }
            }

            return result;
        }

        #endregion
    }
}