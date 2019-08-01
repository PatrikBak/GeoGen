using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// Represents a currently unverified theorem of a certain type that might hold true. 
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
        /// Their actual types depend on the <see cref="Core.TheoremType"/>.
        /// </summary>
        public IEnumerable<GeometricObject> InvolvedObjects { get; set; }

        /// <summary>
        /// Gets or sets the function that is able to tell if the theorem is true in a given picture.
        /// </summary>
        public Func<IPicture, bool> VerificationFunction { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Converts this potential theorem to an actual theorem.
        /// </summary>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <returns>The theorem.</returns>
        public Theorem ToTheorem(Configuration configuration)
        {
            // Map distinct theorem objects to a dictionary
            var theoremObjectsDictionary = InvolvedObjects.Distinct().ToDictionary(key => key, geometricObject =>
            {
                // If we have a point, then we have the only option...
                if (geometricObject is PointObject)
                    return new TheoremPointObject(geometricObject.ConfigurationObject) as TheoremObject;

                // Otherwise the object is either a line, or a circle, so its definable by points
                var objectWithPoints = (DefinableByPoints) geometricObject;

                // Let's find the configuration objects corresponding to these points 
                var points = objectWithPoints.Points.Select(point => point.ConfigurationObject).ToArray();

                // Determine the right type of the theorem object 
                // We're using that it's either a line, or a circle, so 
                // if it's not a line, then it's a circle
                var objectType = objectWithPoints is LineObject
                        ? ConfigurationObjectType.Line
                        : ConfigurationObjectType.Circle;

                // Construct the final theorem object
                return new TheoremObjectWithPoints(objectType, geometricObject.ConfigurationObject, points);
            });

            // Create the theorem objects using the created dictionary
            var theoremObjects = InvolvedObjects.Select(geometricObject => theoremObjectsDictionary[geometricObject]).ToList();

            // Create a new theorem using the created theorem objects
            return new Theorem(configuration, TheoremType, theoremObjects);
        }

        #endregion
    }
}