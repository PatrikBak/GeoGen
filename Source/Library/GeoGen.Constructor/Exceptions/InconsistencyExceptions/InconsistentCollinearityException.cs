using System;
using System.Collections.Generic;

namespace GeoGen.Constructor
{
    /// <summary>
    /// An <see cref="InconsistentPicturesException"/> thrown when collinearities between
    /// the passed points are not the same in every picture.
    /// </summary>
    public class InconsistentCollinearityException : InconsistentPicturesException
    {
        #region Public properties

        /// <summary>
        /// The points that whose collinearity couldn't be determined consistently.
        /// </summary>
        public IReadOnlyList<PointObject> ProblematicPoints { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentCollinearityException"/> class.
        /// </summary>
        /// <param name="problematicPoints">The points that whose collinearity couldn't be determined consistently.</param>
        public InconsistentCollinearityException(IReadOnlyList<PointObject> problematicPoints)
        {
            ProblematicPoints = problematicPoints ?? throw new ArgumentNullException(nameof(problematicPoints));
        }

        #endregion
    }
}