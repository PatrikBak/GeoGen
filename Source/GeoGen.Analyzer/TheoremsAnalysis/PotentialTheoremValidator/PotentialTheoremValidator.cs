using System;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IPotentialTheoremValidator"/>. This
    /// implementation uses the strategy where we want a theorem to be true in all
    /// the containers that are used to represent a given configuration.
    /// </summary>
    public class PotentialTheoremValidator : IPotentialTheoremValidator
    {
        #region Private fields

        /// <summary>
        /// The mapper that finds the containers manager for a configuration.
        /// </summary>
        private readonly IObjectContainersMapper _mapper;

        /// <summary>
        /// The analyzer whether given output would represent a correct theorem
        /// (in case it is true).
        /// </summary>
        private readonly INeedlessObjectsAnalyzer _analyzer;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="mapper">The manager of all objects containers managers.</param>
        /// <param name="analyzer">The analyzer that makes sure that there are no needless objects in theorems.</param>
        public PotentialTheoremValidator(IObjectContainersMapper mapper, INeedlessObjectsAnalyzer analyzer)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
        }

        #endregion

        #region IOutputValidator implementation

        /// <summary>
        /// Finds out if a given potential theorem represents a real theorem
        /// that holds true in a given configuration. 
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="theorem">The theorem.</param>
        /// <returns>true, if the theoem is right; false otherwise.</returns>
        public bool Validate(Configuration configuration, PotentialTheorem theorem)
        {
            // Local function that calls the needless objects analyzer
            bool ContainsNeedlessObjects() => _analyzer.ContainsNeedlessObjects(configuration, theorem.InvolvedObjects);

            // If the verifier function is not specified, then it means the theorem should be 
            // true in all the containers. So it's valid if and only if it doesn't contain needless objects
            if (theorem.VerifierFunction == null)
                return !ContainsNeedlessObjects();

            // Otherwise we first find the containers manager
            var manager = _mapper.Get(configuration);

            // The theorem is true if and only if is true in all containers and doesn't contain needless objects
            return manager.All(theorem.VerifierFunction) && !ContainsNeedlessObjects();
        }

        #endregion
    }
}