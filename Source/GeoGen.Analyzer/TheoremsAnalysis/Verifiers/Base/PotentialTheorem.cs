using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a wrapper for potential theorem that might hold a true. This 
    /// class represents an output from <see cref="ITheoremVerifier"/>.
    /// </summary>
    public class PotentialTheorem
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the type of this possible theorem.
        /// </summary>
        public TheoremType TheoremType { get; set; }

        /// <summary>
        /// Gets or sets the enumerable of the involved geometrical objects in this theorem.
        /// </summary>
        public IEnumerable<GeometricalObject> InvolvedObjects { get; set; }

        /// <summary>
        /// Gets or sets the verifier function that verifies this theorem
        /// in a given container. If the theorem is certainly true, then this
        /// function should be null.
        /// </summary>
        public Func<IObjectsContainer, bool> VerifierFunction { get; set; }

        #endregion

        #region Public methods

        public Theorem ToTheorem()
        {
            // Prepare a local function that casts a geometrical object to theorem object
            TheoremObject Construct(GeometricalObject geometricalObject)
            {
                // First we look if the configuration version of the object is present
                var configurationObject = geometricalObject.ConfigurationObject;

                // If it's present
                if (configurationObject != null)
                    // Then we simply wrap it
                    return new TheoremObject(configurationObject);

                // Otherwise the object is either a line, or a circle, so its definable by points
                var objectWithPoints = (DefinableByPoints) geometricalObject;

                // We pull the points that defines the object
                var points = objectWithPoints.Points;

                // And pull their configuration versions
                var involedObjects = points.Select(p => p.ConfigurationObject).ToArray();

                // Determine the right signature of the theorem object (according to whether
                // it is a line or a circle)
                var objectType = objectWithPoints is LineObject
                        ? TheoremObjectSignature.LineGivenByPoints
                        : TheoremObjectSignature.CircleGivenByPoints;

                // And finally construct the theorem objects
                return new TheoremObject(objectType, involedObjects);
            }

            // Cast all involved objects to the theorem objects
            var theoremObjects = InvolvedObjects.Select(Construct).ToList();

            // Construct the theorem
            return new Theorem(TheoremType, theoremObjects);
        }

        #endregion
    }
}