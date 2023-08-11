using GeoGen.Core;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a rule that explains what objects we should introduce based on the existence of another
    /// constructed objects.
    /// </summary>
    public class ObjectIntroductionRule
    {
        #region Public properties

        /// <summary>
        /// The object that is already there.
        /// </summary>
        public ConstructedConfigurationObject ExistingObject { get; }

        /// <summary>
        /// The list of objects to be introduced. The arguments of the objects must be either the existing object
        /// or its arguments.
        /// </summary>
        public IReadOnlyList<ConstructedConfigurationObject> NewObjects { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectIntroductionRule"/> class.
        /// </summary>
        /// <param name="existingObjects">The object that is already there.</param>
        /// <param name="newObjects">The list of objects to be introduced. The arguments of the objects must be either the existing object or its arguments.</param>
        public ObjectIntroductionRule(ConstructedConfigurationObject existingObject, IReadOnlyList<ConstructedConfigurationObject> newObjects)
        {
            ExistingObject = existingObject ?? throw new ArgumentNullException(nameof(existingObject));
            NewObjects = newObjects ?? throw new ArgumentNullException(nameof(newObjects));
        }

        #endregion
    }
}
