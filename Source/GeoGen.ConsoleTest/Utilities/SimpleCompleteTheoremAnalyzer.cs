using GeoGen.Analyzer;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.ConsoleTest
{
    /// <summary>
    /// Represents a service that finds all theorems that are true in a configuration.
    /// 
    /// NOTE: This is not part of any of the main algorithm. It is here just for the curiosity
    ///       of the programmer of this project. Its implementation is highly inefficient. 
    ///       When it becomes a serious part, it will be improved.
    ///       
    /// </summary>
    public class SimpleCompleteTheoremAnalyzer
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

        /// <summary>
        /// The factory for creating contextual pictures that are required by the relevant theorem analyzer.
        /// </summary>
        private readonly IContextualPictureFactory _factory;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleCompleteTheoremAnalyzer"/> class.
        /// </summary>
        /// <param name="analyzer">The analyzer of relevant theorems that is reused.</param>
        /// <param name="constructor">The constructor of configurations.</param>
        /// <param name="factory">The factory for creating contextual pictures that are required by the relevant theorem analyzer.</param>
        public SimpleCompleteTheoremAnalyzer(IRelevantTheoremsAnalyzer analyzer, IGeometryConstructor constructor, IContextualPictureFactory factory)
        {
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
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
                throw new GeoGenException("The configuration couldn't be examined.");

            // Make sure it is constructible
            if (geometryData.InconstructibleObject != null)
                throw new GeoGenException("The configuration contains inconstructible objects.");

            // Make sure it has no duplicates
            if (geometryData.Duplicates != (null, null))
                throw new GeoGenException("The configurations contains duplicate objects.");

            #endregion

            // We find theorems in every correct sub-configuration of this
            // Take all the subjects of the constructed objects
            var theorems = configuration.ConstructedObjects.Subsets()
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
                .Select(_configuration =>
                {
                    try
                    {
                        // Safely create the picture
                        var picture = _factory.Create(_configuration, geometryData.Manager);

                        // Run the analysis
                        return _analyzer.Analyze(_configuration, geometryData.Manager, picture);
                    }
                    catch (InconstructibleContextualPicture)
                    {
                        // If we cannot construct this picture, we can't have full results...
                        throw new GeoGenException("There is a configuration for which we were not able to create a contextual picture");
                    }
                })
                // Enumerate for further processing
                .ToList();

            // Local function to take theorems ordered by type for prettier result
            List<AnalyzedTheorem> Order(IEnumerable<AnalyzedTheorem> input) => input.Distinct(Theorem.EquivalencyComparer).Cast<AnalyzedTheorem>().OrderBy(t => t.Type).ToList();

            // Create a single output that merges the results
            return new TheoremAnalysisOutput
            {
                // Merge all the theorems
                Theorems = Order(theorems.SelectMany(output => output.Theorems)),

                // Merge all the potential false negatives
                PotentialFalseNegatives = Order(theorems.SelectMany(output => output.PotentialFalseNegatives))
            };
        }

        #endregion
    }
}
