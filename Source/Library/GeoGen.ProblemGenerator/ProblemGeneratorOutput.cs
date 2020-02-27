using GeoGen.ConfigurationGenerator;
using GeoGen.Constructor;
using GeoGen.Core;
using System;

namespace GeoGen.ProblemGenerator
{
    /// <summary>
    /// Represents an output of the <see cref="IProblemGenerator"/>.
    /// </summary>
    public class ProblemGeneratorOutput
    {
        #region Public properties

        /// <summary>
        /// The generated configuration.
        /// </summary>
        public GeneratedConfiguration Configuration { get; }

        /// <summary>
        /// The contextual picture where the configuration is drawn.
        /// </summary>
        public ContextualPicture ContextualPicture { get; }

        /// <summary>
        /// The found theorems for the configurations that don't use the last object of the configuration.
        /// </summary>
        public TheoremMap OldTheorems { get; }

        /// <summary>
        /// The found theorems for the configurations that use the last object of the configuration.
        /// </summary>
        public TheoremMap NewTheorems { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemGeneratorOutput"/> class.
        /// </summary>
        /// <param name="configuration">The generated configuration.</param>
        /// <param name="contextualPicture">The contextual picture where the configuration is drawn.</param>
        /// <param name="oldTheorems">The found theorems for the configurations that don't use the last object of the configuration.</param>
        /// <param name="newTheorems">The found theorems for the configurations that use the last object of the configuration.</param>
        public ProblemGeneratorOutput(GeneratedConfiguration configuration, ContextualPicture contextualPicture, TheoremMap oldTheorems, TheoremMap newTheorems)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            ContextualPicture = contextualPicture ?? throw new ArgumentNullException(nameof(contextualPicture));
            OldTheorems = oldTheorems ?? throw new ArgumentNullException(nameof(oldTheorems));
            NewTheorems = newTheorems ?? throw new ArgumentNullException(nameof(newTheorems));
        }

        #endregion
    }
}