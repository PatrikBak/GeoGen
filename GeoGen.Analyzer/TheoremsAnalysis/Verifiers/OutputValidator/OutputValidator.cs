using System;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IOutputValidator"/>.
    /// </summary>
    internal class OutputValidator : IOutputValidator
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
        public OutputValidator(IObjectContainersMapper mapper, INeedlessObjectsAnalyzer analyzer)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
        }

        #endregion

        #region IOutputValidator implementation

        /// <summary>s
        /// Finds out if a given verifier output represents a true and acceptable
        /// theorem for a configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="output">The output.</param>
        /// <returns>true, if the output represents a correct acceptable theorem; false otherwise.</returns>
        public bool Validate(Configuration configuration, VerifierOutput output)
        {
            // Local function that calls the needless objects analyzer
            bool ContainsNeedlessObjects() => _analyzer.ContainsNeedlessObjects(configuration, output.InvoldedObjects);

            // If the output is always true, then it's fine if and if only if it doesn't contain needless objects
            if (output.AlwaysTrue)
                return !ContainsNeedlessObjects();

            // Otherwise we first find the containers manager
            var manager = _mapper.Get(configuration);

            // The theorem is true if and only if is true in all containers and doesn't contain needless objects
            return manager.All(output.VerifierFunction) && !ContainsNeedlessObjects();
        }

        #endregion
    }
}