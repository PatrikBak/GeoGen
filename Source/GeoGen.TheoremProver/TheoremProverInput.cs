using GeoGen.Constructor;
using GeoGen.Core;
using System;
using System.Linq;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents an input for a <see cref="ITheoremProver"/>.
    /// </summary>
    public class TheoremProverInput
    {
        #region Public properties

        /// <summary>
        /// The contextual picture where the configuration is drawn.
        /// </summary>
        public ContextualPicture ContextualPicture { get; }

        /// <summary>
        /// The theorems that hold true in the configuration and cannot be stated without its last object. 
        /// These are the theorems that will be analyzed whether they are interesting or not.
        /// </summary>
        public TheoremsMap NewTheorems { get; }

        /// <summary>
        /// The theorems that hold true in the configuration and can be stated without its last object.
        /// These are the theorems are assumed to already have been analyzed, i.e. not interesting.
        /// </summary>
        public TheoremsMap OldTheorems { get; }

        /// <summary>
        /// The theorems that hold true in the configuration.
        /// </summary>
        public TheoremsMap AllTheorems { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremProverInput"/> class.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture where the configuration is drawn.</param>
        /// <param name="newTheorems">The theorems that hold true in the configuration and cannot be stated without its last object.</param>
        /// <param name="oldTheorems">The theorems that hold true in the configuration and can be stated without its last object.</param>
        public TheoremProverInput(ContextualPicture contextualPicture, TheoremsMap newTheorems, TheoremsMap oldTheorems)
        {
            ContextualPicture = contextualPicture ?? throw new ArgumentNullException(nameof(contextualPicture));
            NewTheorems = newTheorems ?? throw new ArgumentNullException(nameof(newTheorems));
            OldTheorems = oldTheorems ?? throw new ArgumentNullException(nameof(oldTheorems));

            // Create the map of all theorems by merging the old and new ones
            AllTheorems = new TheoremsMap(OldTheorems.AllObjects.Concat(NewTheorems.AllObjects));
        }

        #endregion
    }
}
