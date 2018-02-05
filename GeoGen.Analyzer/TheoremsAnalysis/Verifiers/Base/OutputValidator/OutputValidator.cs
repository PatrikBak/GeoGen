using System;
using System.Linq;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IOutputValidator"/>.
    /// </summary>
    internal class OutputValidator : IOutputValidator
    {
        #region Private fields

        /// <summary>
        /// The manager holding all objects containers.
        /// </summary>
        private readonly IObjectsContainersManager _manager;

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
        /// <param name="manager">The manager of objects containers.</param>
        /// <param name="analyzer">The analyzer that makes sure that there are no needless objects.</param>
        public OutputValidator(IObjectsContainersManager manager, INeedlessObjectsAnalyzer analyzer)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
        }

        #endregion

        #region IOutputValidator implementation

        /// <summary>
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

            // If the output is always true, then its fine if and if only if doesn't contain needless objects
            if (output.AlwaysTrue)
                return !ContainsNeedlessObjects();

            // Otherwise the theorem is true if and only if is true in all containers
            // and doesn't contain needless objects
            return _manager.AtLeast(10, output.VerifierFunction) && !ContainsNeedlessObjects();
        }

        #endregion
    }
}