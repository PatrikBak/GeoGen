using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// An <see cref="InconsistentPicturesException"/> thrown when constructibility of
    /// the passed object is not the same in every picture.
    /// </summary>
    public class InconsistentConstructibilityException : InconsistentPicturesException
    {
        #region Public properties

        /// <summary>
        /// The point whose constructibility couldn't be determined consistently.
        /// </summary>
        public ConstructedConfigurationObject ConstructedObject { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentConstructibilityException"/> class.
        /// </summary>
        /// <param name="constructedObject">The point whose constructibility couldn't be determined consistently.</param>
        public InconsistentConstructibilityException(ConstructedConfigurationObject constructedObject)
        {
            ConstructedObject = constructedObject ?? throw new ArgumentNullException(nameof(constructedObject));
        }

        #endregion
    }
}