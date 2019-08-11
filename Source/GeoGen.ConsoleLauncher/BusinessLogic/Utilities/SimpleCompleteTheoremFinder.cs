using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.TheoremsFinder;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Simple default implementation of <see cref="ICompleteTheoremsFinder"/>.
    /// 
    /// <para>
    /// NOTE: This is not part of any of the main algorithm. It is here just for the curiosity
    ///       of the programmer of this project. Its implementation is highly inefficient. 
    ///       When it becomes a serious part, it will be improved.
    /// </para>
    /// </summary>
    public class SimpleCompleteTheoremFinder : ICompleteTheoremsFinder
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
        /// Initializes a new instance of the <see cref="SimpleCompleteTheoremFinder"/> class.
        /// </summary>
        /// <param name="analyzer">The analyzer of relevant theorems that is reused.</param>
        /// <param name="constructor">The constructor of configurations.</param>
        /// <param name="factory">The factory for creating contextual pictures that are required by the relevant theorem analyzer.</param>
        public SimpleCompleteTheoremFinder(IRelevantTheoremsAnalyzer analyzer, IGeometryConstructor constructor, IContextualPictureFactory factory)
        {
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #endregion

        #region ICompleteTheoremsFinder implementation

        /// <summary>
        /// Finds all the theorems that are true in a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The theorems.</returns>
        public List<Theorem> FindAllTheorems(Configuration configuration)
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
            return configuration.ConstructedObjects.Subsets()
                // Take only those that make a correct configuration
                .Where(objects =>
                {
                    // We will find this out by having look at the number of distinct 
                    // objects of the potential configuration
                    var numberOfObjects = configuration.LooseObjects.Cast<ConfigurationObject>().GetDefiningObjects().Count();

                    // There shouldn't be any leftover objects
                    return numberOfObjects <= configuration.LooseObjects.Count + objects.ToArray().Length;
                })
                // Make a configuration. The constructed objects should be ordered correctly
                .Select(objects => new Configuration(configuration.LooseObjectsHolder, objects.ToArray()))
                // Select only distinct ones
                .Distinct(new SimpleEqualityComparer<Configuration>((c1, c2) => c1.AllObjects.ToSet().SetEquals(c2.AllObjects)))
                // Find its theorems
                .Select(_configuration =>
                {
                    try
                    {
                        // Create a contextual picture
                        var picture = _factory.Create(_configuration.AllObjects, geometryData.Manager);

                        // Run the analysis
                        return _analyzer.Analyze(_configuration, geometryData.Manager, picture);
                    }
                    catch (InconstructibleContextualPicture)
                    {
                        // If we cannot construct this picture, we can't have full results...
                        throw new GeoGenException("There is a configuration for which we were not able to create a contextual picture");
                    }
                })
                // Flatten all these outputs
                .Flatten()
                // Exclude equal ones
                .Distinct(Theorem.EquivalencyComparer)
                // Order them by type
                .OrderBy(theorem => theorem.Type)
                // Enumerate to a list
                .ToList();
        }

        #endregion
    }
}
