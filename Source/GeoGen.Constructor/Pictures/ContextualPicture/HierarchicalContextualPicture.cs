using System;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a <see cref="ContextualPicture"/> that was created by cloning
    /// another contextual picture and adding an object to it.
    /// </summary>
    public class HierarchicalContextualPicture : ContextualPicture
    {
        #region Public properties

        /// <summary>
        /// Gets the original picture from which this contextual picture was created.
        /// </summary>
        public ContextualPicture OriginalPicture { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchicalContextualPicture"/> class.
        /// </summary>
        /// <param name="originalPicture">The original picture from which this contextual picture was created.</param>
        /// <param name="pictures">The pictures that hold all the representations of the configuration.</param>
        /// <param name="tracer">The tracer of unsuccessful attempts to reconstruct the contextual picture.</param>
        public HierarchicalContextualPicture(ContextualPicture originalPicture, Pictures pictures, IContexualPictureConstructionFailureTracer tracer = null)
            : base(pictures, addObjects: false, tracer)
        {
            OriginalPicture = originalPicture ?? throw new ArgumentNullException(nameof(originalPicture));
        }

        #endregion
    }
}
