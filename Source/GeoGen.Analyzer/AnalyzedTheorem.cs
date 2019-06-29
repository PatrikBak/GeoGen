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
        /// Gets the number of objects containers in which the theorem was true after the first test.
        /// </summary>
        public int NumberOfTrueContainersAfterFirstTest { get; }

        /// <summary>
        /// Gets  the number of objects containers in which the theorem was true after the second test.
        /// This value may be null if there wasn't the second test, or if it wasn't successful.
        /// </summary>
        public int? NumberOfTrueContainersAfterSecondTest { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzedTheorem"/> object 
        /// from a potential theorem and analysis data.
        /// </summary>
        /// <param name="configuration">The configuration in which the theorem holds.</param>
        /// <param name="potentialTheorem">The potential theorem that was analyzed.</param>
        /// <param name="numberOfTrueContainersAfterFirstTest">The number of objects containers in which the theorem was true after the first test.</param>
        /// <param name="numberOfTrueContainersAfterSecondTest">Gets  the number of objects containers in which the theorem was true after the second test, or null, if there wasn't any.</param>
        public AnalyzedTheorem(Configuration configuration, PotentialTheorem potentialTheorem, int numberOfTrueContainersAfterFirstTest, int? numberOfTrueContainersAfterSecondTest)
            : base(configuration, potentialTheorem.TheoremType, ConstructTheoremObjects(potentialTheorem.InvolvedObjects))
        {
            NumberOfTrueContainersAfterFirstTest = numberOfTrueContainersAfterFirstTest;
            NumberOfTrueContainersAfterSecondTest = numberOfTrueContainersAfterSecondTest;
        }

        #endregion

        #region Private static methods

        /// <summary>
        /// Constructs a list of theorem objects from geometrical objects.
        /// </summary>
        /// <param name="geometricalObjects">The geometrical objects that will be converted to theorem objects.</param>
        /// <returns>The list of theorem objects representing the given geometrical objects.</returns>
        private static IReadOnlyList<TheoremObject> ConstructTheoremObjects(IEnumerable<GeometricalObject> geometricalObjects)
        {
            // Local function that converts a geometrical object to a theorem object
            TheoremObject Construct(GeometricalObject geometricalObject)
            {
                // If we have a point, then we have the only option...
                if (geometricalObject is PointObject)
                    return new TheoremPointObject(geometricalObject.ConfigurationObject);

                // Otherwise the object is either a line, or a circle, so its definable by points
                var objectWithPoints = (DefinableByPoints)geometricalObject;

                // Let's find the configuration objects corresponding to these points 
                var points = objectWithPoints.Points.Select(point => point.ConfigurationObject).ToArray();

                // Determine the right type of the theorem object 
                // We're using that it's either a line, or a circle, so 
                // if it's not a line, then it's a circle
                var objectType = objectWithPoints is LineObject
                        ? ConfigurationObjectType.Line
                        : ConfigurationObjectType.Circle;

                // Construct the final theorem object
                return new TheoremObjectWithPoints(objectType, geometricalObject.ConfigurationObject, points);
            }

            // Convert all the involved objects to theorem objects
            return geometricalObjects.Select(Construct).ToList();
        }

        #endregion
    }
}
