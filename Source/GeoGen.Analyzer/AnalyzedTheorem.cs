using GeoGen.Constructor;
using GeoGen.Core;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a <see cref="Theorem"/> together with the analysis results. 
    /// </summary>
    public class AnalyzedTheorem : Theorem
    {
        #region Public properties

        /// <summary>
        /// Gets the number of pictures in which the theorem was true after the first test.
        /// </summary>
        public int NumberOfTruePicturesAfterFirstTest { get; }

        /// <summary>
        /// Gets  the number of pictures in which the theorem was true after the second test.
        /// This value may be null if there wasn't the second test, or if it wasn't successful.
        /// </summary>
        public int? NumberOfTruePicturesAfterSecondTest { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzedTheorem"/> object 
        /// from a potential theorem and analysis data.
        /// </summary>
        /// <param name="configuration">The configuration in which the theorem holds.</param>
        /// <param name="potentialTheorem">The potential theorem that was analyzed.</param>
        /// <param name="numberOfTruePicturesAfterFirstTest">The number of pictures in which the theorem was true after the first test.</param>
        /// <param name="numberOfTruePicturesAfterSecondTest">Gets  the number of pictures in which the theorem was true after the second test, or null, if there wasn't any.</param>
        public AnalyzedTheorem(Configuration configuration, PotentialTheorem potentialTheorem, int numberOfTruePicturesAfterFirstTest, int? numberOfTruePicturesAfterSecondTest)
            : base(configuration, potentialTheorem.TheoremType, ConstructTheoremObjects(potentialTheorem.InvolvedObjects))
        {
            NumberOfTruePicturesAfterFirstTest = numberOfTruePicturesAfterFirstTest;
            NumberOfTruePicturesAfterSecondTest = numberOfTruePicturesAfterSecondTest;
        }

        #endregion

        #region Private static methods

        /// <summary>
        /// Constructs a list of theorem objects from geometric objects.
        /// </summary>
        /// <param name="geometricObjects">The geometric objects that will be converted to theorem objects.</param>
        /// <returns>The list of theorem objects representing the given geometric objects.</returns>
        private static IReadOnlyList<TheoremObject> ConstructTheoremObjects(IEnumerable<GeometricObject> geometricObjects)
        {
            // Convert all the involved objects to theorem objects
            return geometricObjects.Select(geometricObject =>
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
            })
            // Enumerate to a list
            .ToList();
        }

        #endregion
    }
}
