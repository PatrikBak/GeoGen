using GeoGen.Core;
using System;
using System.Collections.Generic;

namespace GeoGen.Constructor
{
    /// <summary>
    /// An <see cref="InconsistentPicturesException"/> thrown when the passed object in not
    /// equal to the other passed objects in every picture.
    /// </summary>
    public class InconsistentEqualityException : InconsistentPicturesException
    {
        #region Public properties

        /// <summary>
        /// The new object that couldn't be examined consistently.
        /// </summary>
        public ConfigurationObject ConstructedObject { get; }

        /// <summary>
        /// The configuration objects that were inconsistently evaluated equal to the examined object. 
        /// Either this value or <see cref="EqualGeometricObjects"/> is not null.
        /// </summary>
        public IReadOnlyList<ConfigurationObject> EqualObjects { get; }

        /// <summary>
        /// The geometric objects that were inconsistently evaluated equal to the examined object.
        /// Either this value or <see cref="EqualObjects"/> is not null.
        /// </summary>
        public IReadOnlyList<GeometricObject> EqualGeometricObjects { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentEqualityException"/> class.
        /// </summary>
        /// <param name="configurationObject">The new object that couldn't be examined consistently.</param>
        /// <param name="equalObjects">The configuration objects that were inconsistently evaluated equal to the examined object.</param>
        public InconsistentEqualityException(ConfigurationObject configurationObject, IReadOnlyList<ConfigurationObject> equalObjects)
        {
            ConstructedObject = configurationObject ?? throw new ArgumentNullException(nameof(configurationObject));
            EqualObjects = equalObjects ?? throw new ArgumentNullException(nameof(equalObjects));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentEqualityException"/> class.
        /// </summary>
        /// <param name="configurationObject">The new object that couldn't be examined consistently.</param>
        /// <param name="equalGeometricObjects">The geometric objects that were inconsistently evaluated equal to the examined object.</param>
        public InconsistentEqualityException(ConfigurationObject configurationObject, IReadOnlyList<GeometricObject> equalGeometricObjects)
        {
            ConstructedObject = configurationObject ?? throw new ArgumentNullException(nameof(configurationObject));
            EqualGeometricObjects = equalGeometricObjects ?? throw new ArgumentNullException(nameof(equalGeometricObjects));
        }

        #endregion
    }
}