using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a simple <see cref="ICompleteTheoremAnalyzer"/> that reuses 
    /// <see cref="IRelevantTheoremsAnalyzer"/> in a correct, but inefficient way.
    /// It should not be used to analyze configurations in bulk.
    /// </summary>
    public class SimpleCompleteTheoremAnalyzer : ICompleteTheoremAnalyzer
    {
        #region Dependencies

        /// <summary>
        /// The analyzer of relevant theorems that is reused.
        /// </summary>
        private IRelevantTheoremsAnalyzer _analyzer;

        /// <summary>
        /// The constructor of configurations.
        /// </summary>
        private IGeometryConstructor _constructor;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCompleteTheoremAnalyzer"/> class.
        /// </summary>
        /// <param name="analyzer">The analyzer of relevant theorems that is reused.</param>
        /// <param name="constructor">The constructor of configurations.</param>
        public SimpleCompleteTheoremAnalyzer(IRelevantTheoremsAnalyzer analyzer, IGeometryConstructor constructor)
        {
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        #endregion

        #region ICompleteTheoremAnalyzer implementation

        /// <summary>
        /// Finds all the theorems that are true in a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The analysis output.</returns>
        public TheoremAnalysisOutput Analyze(Configuration configuration)
        {
            #region Construction

            // Construct the configuration
            var geometryData = _constructor.Construct(configuration);

            // Make sure it's correct
            if (!geometryData.SuccessfullyExamined)
                throw new AnalyzerException("The configuration couldn't be examined.");

            // Make sure it is constructible
            if (geometryData.InconstructibleObject != null)
                throw new AnalyzerException("The configuration contains inconstructible objects.");

            // Make sure it has no duplicates
            if (geometryData.Duplicates != (null, null))
                throw new AnalyzerException("The configurations contains duplicate objects.");

            #endregion

            // We find theorems in every correct sub-configuration of this
            // Take all the subjects of the constructed objects
            var theorems = configuration.ConstructedObjects.Subsets()
                // Enumerate each sorted by id
                //.Select(objects => objects.OrderBy(obj => obj.Id).ToArray())
                // Take only those that make a correct configuration
                .Where(objects =>
                {
                    // We will find this out by having look at the number of distinct 
                    // objects of the potential configuration
                    var numberOfObjects = configuration.LooseObjectsHolder.LooseObjects.Cast<ConfigurationObject>().GetDefiningObjects().Count();

                    // There shouldn't be any leftover objects
                    return numberOfObjects <= configuration.LooseObjectsHolder.LooseObjects.Count + objects.ToArray().Length;
                })
                // Make a configuration. The constructed objects should be ordered correctly
                .Select(objects => new Configuration(configuration.LooseObjectsHolder, objects.ToArray()))
                // Find its theorems
                .Select(_configuration => _analyzer.Analyze(_configuration, geometryData.Manager))
                // Enumerate for further processing
                .ToList();

            // Make sure we have all the results
            if (theorems.Any(_output => !_output.TheoremAnalysisSuccessful))
                throw new AnalyzerException("The theorem analysis wasn't successful");

            // Local function to take theorems ordered by type for prettier result
            // TODO: Handle duplicates
            List<AnalyzedTheorem> Order(IEnumerable<AnalyzedTheorem> input) => input.OrderBy(t => t.Type).ToList();

            // Create a single output that merges the results
            return new TheoremAnalysisOutput
            {
                // It went fine
                TheoremAnalysisSuccessful = true,

                // Merge all the theorems
                Theorems = Order(theorems.SelectMany(output => output.Theorems)),

                // Merge all the potential false negatives
                PotentialFalseNegatives = Order(theorems.SelectMany(output => output.PotentialFalseNegatives))
            };
        }

        #endregion
    }
}
