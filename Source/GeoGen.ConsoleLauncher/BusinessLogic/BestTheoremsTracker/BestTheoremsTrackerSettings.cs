using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The settings for <see cref="BestTheoremsTracker"/>.
    /// </summary>
    public class BestTheoremsTrackerSettings
    {
        #region Public properties

        /// <summary>
        /// The file where the theorems will be written to. 
        /// </summary>
        public string TheoremFilePath { get; }

        /// <summary>
        /// The maximal number of theorems that will be written to the file with best theorems.
        /// </summary>
        public int NumberOfTheorems { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BestTheoremsTrackerSettings"/> class.
        /// </summary>
        /// <param name="theoremFilePath">The file where the theorems will be written to. </param>
        /// <param name="numberOfTheorems">The maximal number of theorems that will be written to the file with best theorems.</param>
        public BestTheoremsTrackerSettings(string theoremFilePath, int numberOfTheorems)
        {
            TheoremFilePath = theoremFilePath ?? throw new ArgumentNullException(nameof(theoremFilePath));
            NumberOfTheorems = numberOfTheorems;
        }

        #endregion
    }
}