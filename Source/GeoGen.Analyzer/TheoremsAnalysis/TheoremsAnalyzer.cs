using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="ITheoremsAnalyzer"/>.
    /// </summary>
    internal class TheoremsAnalyzer : ITheoremsAnalyzer
    {
        #region Private fields

        /// <summary>
        /// The array of all available theorem verifiers.
        /// </summary>
        private readonly ITheoremVerifier[] _verifiers;

        /// <summary>
        /// The factory for creating contextual containers
        /// </summary>
        private readonly IContextualContainerFactory _factory;

        /// <summary>
        /// The validator of an output of verifiers.
        /// </summary>
        private readonly IPotentialTheoremValidator _outputValidator;

        /// <summary>
        /// The container of default theorems.
        /// </summary>
        private readonly ITheoremsContainer _container;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="verifiers">The array of available verifiers to be used.</param>
        /// <param name="factory">The factory for creating contextual containers holding configurations.</param>
        /// <param name="container">The container of default theorems.</param>
        /// <param name="validator">The validator of verifiers output.</param>
        public TheoremsAnalyzer
        (
            ITheoremVerifier[] verifiers,
            IContextualContainerFactory factory,
            IPotentialTheoremValidator validator,
            ITheoremsContainer container
        )
        {
            _verifiers = verifiers ?? throw new ArgumentNullException(nameof(verifiers));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _outputValidator = validator ?? throw new ArgumentNullException(nameof(validator));
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        #endregion

        #region ITheoremsAnalyzer implementation

        /// <summary>
        /// Performs the theorem analysis for a given configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The list of theorems that hold true in the configuration.</returns>
        public List<Theorem> Analyze(Configuration configuration)
        {
            // Create contextual container holding the configuration
            var container = _factory.Create(configuration);

            // For each verifier create many outputs
            return _verifiers.SelectMany(verifier => verifier.FindPotencialTheorems(container))
                    // Take only those that represent a correct theorem
                    .Where(output => _outputValidator.Validate(configuration, output))
                    // Cast them to actual theorems
                    .Select(output => ConstructTheorem(output.TheoremType, output.InvolvedObjects))
                    // Take only those that are not present in the container of default theorems
                    .Where(theorem => !_container.Contains(theorem))
                    // Enumerate to list
                    .ToList();
        }

        /// <summary>
        /// Constructs a theorem where each geometrical object represents one theorem object.
        /// </summary>
        /// <param name="type">The type of the theorem to construct.</param>
        /// <param name="objects">The geometrical objects.</param>
        /// <returns>The theorem.</returns>
        private Theorem ConstructTheorem(TheoremType type, IEnumerable<GeometricalObject> objects)
        {
            // Prepare a local function that casts a geometrical object to theorem bject
            TheoremObject Construct(GeometricalObject geometricalObject)
            {
                // First we look if the configuration version of the object is present
                var configurationObject = geometricalObject.ConfigurationObject;

                // If it's present
                if (configurationObject != null)
                    // Then we simply wrap it
                    return new TheoremObject(configurationObject);

                // Otherwise the object is either a line, or a circle, so its definable by points
                var objectWithPoints = (DefinableByPoints)geometricalObject;

                // We pull the points that defines the object
                var points = objectWithPoints.Points;

                // And pull their configuration versions
                var involedObjects = points.Select(p => p.ConfigurationObject).ToArray();

                // Determine the right signature of the theorem object (according to whether
                // it is a line or a circle)
                var objectType = objectWithPoints is LineObject
                        ? TheoremObjectSignature.LineGivenByPoints
                        : TheoremObjectSignature.CircleGivenByPoints;

                // And finally construct the theorem objects
                return new TheoremObject(objectType, involedObjects);
            }

            // Cast all involved objects to the theorem objects
            var theoremObjects = objects.Select(Construct).ToList();

            // Construct the theorem
            return new Theorem(type, theoremObjects);
        }

        #endregion
    }
}