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
        /// Finds out if the theorem can be defined using fewer objects than a given expected number.
        /// There might be more ways, because <see cref="GeometricObject"/>s can be defined in 
        /// multiple ways. For example, if we have the theorem that lines [A,B,C] and [A,D,E] are 
        /// perpendicular, where A,B,C,D,E are points, then each of these lines can be defined in 
        /// exactly 3 ways, so there are 9 ways to state this theorem. There might also be more ways
        /// if there was a configuration object representing that line as well, i.e. if it wasn't 
        /// defined implicitly from points. This method takes into account the fact that
        /// configuration objects might be defined by some other objects. For example, if we had 
        /// loose points A,B,C and a constructed object D that uses 2 other constructed objects 
        /// in its definition (and some of the loose points), then in order to define the theorem
        /// stating that AB is perpendicular to CD we would need 6 objects (A,B,C,D + 2 other ones).
        /// </summary>
        /// <param name="expectedMinimalNumberOfNeededObjects">The expected minimal number of configuration 
        /// objects that are required to state this theorem including all the definitions of all the used objects.</param>
        /// <returns>true, if there are fewer needed objects than expected; false otherwise.</returns>
        public bool ContainsNeedlessObjects(int expectedMinimalNumberOfNeededObjects)
        {
            // Local function that enumerated all possible definitions of a given geometric object
            IEnumerable<IEnumerable<ConfigurationObject>> AllDefinitions(GeometricObject geometricObject)
            {
                // Pull configuration object version
                var configurationObject = geometricObject.ConfigurationObject;

                // If the configuration version is set, then the objects defining this one is one possible definition
                if (configurationObject != null)
                    yield return configurationObject.GetDefiningObjects();

                // If this is a point, then there is no other definition
                if (geometricObject is PointObject)
                    yield break;

                // Otherwise we have a line or a circle, i.e. something definable by points
                // Let's find how many of them we need
                var definableByPoints = (DefinableByPoints) geometricObject;

                // Each pair or triple (given by the object's property) together with internal objects is a definition
                // If there are not enough points, we can't do much
                if (definableByPoints.Points.Count < definableByPoints.NumberOfNeededPoints)
                    yield break;

                // Otherwise we have some definition of this type
                // Let's prepare them. First we take all the subsets (all pairs / triples) of points
                var remainingdefinitions = definableByPoints.Points.Subsets(definableByPoints.NumberOfNeededPoints)
                    // For each of these subjects we have a definition
                    .Select(points => points.Select(p => p.ConfigurationObject).GetDefiningObjects());

                // And return them
                foreach (var definition in remainingdefinitions)
                    yield return definition;
            }

            // Now we're finally ready to combine all possible definitions of particular objects
            // First take distinct involved objects
            return InvolvedObjects.Distinct()
                // For each object find all the definitions
                .Select(AllDefinitions)
                // Combine them into a single one in every possible ways
                .Combine()
                // We have needless objects if and only if there is a definition
                // containing fewer objects than we're expecting
                .Any(definition => definition.Flatten().Distinct().Count() < expectedMinimalNumberOfNeededObjects);
        }

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