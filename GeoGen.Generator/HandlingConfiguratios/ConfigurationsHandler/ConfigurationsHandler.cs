using System;
using System.Collections.Generic;
using GeoGen.Analyzer;
using GeoGen.Core.Generator;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// A default implementation of <see cref="IConfigurationsHandler"/>.
    /// </summary>
    internal class ConfigurationsHandler : IConfigurationsHandler
    {
        #region Private fields

        /// <summary>
        /// The gradual analyzer.
        /// </summary>
        private readonly IGradualAnalyzer _analyzer;

        /// <summary>
        /// The configurations container.
        /// </summary>
        private readonly IConfigurationsManager _manager;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a handler that calls a give analyzer for generated
        /// configurations and registers incorrectly defined objects to a given
        /// configurations container.
        /// </summary>
        /// <param name="analyzer">The gradual analyzer.</param>
        /// <param name="manager">The configurations container.</param>
        public ConfigurationsHandler(IGradualAnalyzer analyzer, IConfigurationsManager manager)
        {
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        #endregion

        #region IConfigurationsHandler methods

        /// <summary>
        /// Handles generated configuration. It is supposed to call the analyzer
        /// service and handle it's output.
        /// </summary>
        /// <param name="configurations">The configurations enumerable.</param>
        /// <returns>The final generator output enumerable.</returns>
        public IEnumerable<GeneratorOutput> GenerateFinalOutput(IEnumerable<ConfigurationWrapper> configurations)
        {
            if (configurations == null)
                throw new ArgumentNullException(nameof(configurations));

            // Iterate over configurations
            foreach (var configurationWrapper in configurations)
            {
                // Pull old objects
                var oldObjects = configurationWrapper.OriginalObjects;

                // Pull new objects
                var newObjects = configurationWrapper.LastAddedObjects;

                // Call the analyzer
                var analyzerOutput = _analyzer.Analyze(oldObjects, newObjects);               

                // Pull the information if we should exclude the configuration and the new objects
                var shouldBeExcluded = !analyzerOutput.UnambiguouslyConstructible;

                // If yes, then we do it
                if (shouldBeExcluded)
                {
                    // Set the excluded flag to true
                    configurationWrapper.Excluded = true;

                    // Forbid all new objects
                    foreach (var newObject in newObjects)
                    {
                        _manager.ForbidConfigurationsContaining(newObject);
                    }
                }

                // Pull theorems
                var theorems = analyzerOutput.Theorems;

                // If there is no interesting theorem, continue
                if (theorems.Empty())
                    continue;

                // Otherwise yield the generator output
                yield return new GeneratorOutput
                {
                    Configuration = configurationWrapper.Configuration,
                    Theorems = theorems
                };
            }
        }

        #endregion
    }
}