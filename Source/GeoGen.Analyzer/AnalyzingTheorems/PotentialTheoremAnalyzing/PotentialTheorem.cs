using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a yet unverified theorem of a certain type that might hold true. 
    /// This object is an output of <see cref="IPotentialTheoremsAnalyzer"/>s. 
    /// </summary>
    public class PotentialTheorem
    {
        #region Public properties

        /// <summary>
        /// Gets or sets the type of this potential theorem.
        /// </summary>
        public TheoremType TheoremType { get; set; }

        /// <summary>
        /// Gets or sets the enumerable of the geometric objects that this theorem is about. 
        /// Their actual types depend on the <see cref="TheoremType"/>.
        /// </summary>
        public IEnumerable<GeometricalObject> InvolvedObjects { get; set; }

        /// <summary>
        /// Gets or sets the function that is able to tell if the theorem is true in a given container.
        /// </summary>
        public Func<IObjectsContainer, bool> VerificationFunction { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Converts this potential theorem to an actual theorem. The signatures of the internal
        /// theorem objects are inferred automatically.
        /// </summary>
        /// <returns>The converted theorem.</returns>
        public Theorem ToTheorem()
        {
            // Local function that converts a geometrical object to a theorem object
            TheoremObject Construct(GeometricalObject geometricalObject)
            {
                // First we look if the configuration object version of this object is present
                var configurationObject = geometricalObject.ConfigurationObject;

                // If it's present, then we simply wrap it
                if (configurationObject != null)
                    return new TheoremObject(configurationObject);

                // Otherwise the object is either a line, or a circle, so its definable by points
                var objectWithPoints = (DefinableByPoints) geometricalObject;

                // Let's find the configuration objects corresponding to these points 
                var involedObjects = objectWithPoints.Points.Select(point => point.ConfigurationObject).ToArray();

                // Determine the right signature of the theorem object 
                // We're using that it's either a line, or a circle, so 
                // if it's not a line, then it's a circle
                var objectType = objectWithPoints is LineObject
                        ? TheoremObjectSignature.LineGivenByPoints
                        : TheoremObjectSignature.CircleGivenByPoints;

                // Construct the final theorem object
                return new TheoremObject(objectType, involedObjects);
            }

            // Convert all the involved objects to theorem objects
            var theoremObjects = InvolvedObjects.Select(Construct).ToList();

            // Construct the final result
            return new Theorem(TheoremType, theoremObjects);
        }

        #endregion
    }
}