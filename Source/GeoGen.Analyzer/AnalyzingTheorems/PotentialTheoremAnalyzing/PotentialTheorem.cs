using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
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

                // Otherwise we can't have a point, since it can't be defined otherwise
                // The object is either a line, or a circle, so its definable by points
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

        /// <summary>
        /// Finds out if the theorem can be defined using fewer objects than a given expected number.
        /// There might be more ways, because <see cref="GeometricalObject"/>s can be defined in 
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
            // Helper function that creates a set containing the definition objects 
            // of an configuration object, together with the given object
            HashSet<ConfigurationObject> ObjectWithItsInternalObjects(ConfigurationObject configurationObject)
            {
                // Take the object and concatenate its internal objects
                return configurationObject.GetInternalObjects().Concat(configurationObject).ToSet();
            }

            // Local function that enumerated all possible definitions of a given geometrical object
            // Each definition is given as a set of configuration objects
            IEnumerable<HashSet<ConfigurationObject>> AllDefinitions(GeometricalObject geometricalObject)
            {
                // Pull configuration object version
                var configurationObject = geometricalObject.ConfigurationObject;

                // If the configuration version is set, then it's this object 
                // with its internal ones is one possible definition
                if (configurationObject != null)
                    yield return ObjectWithItsInternalObjects(configurationObject);

                // If this is a point, then there is no other definition
                if (geometricalObject is PointObject)
                    yield break;

                // Otherwise we have a line or a circle, i.e. something definable by points
                // Let's find how many of them we need
                var definableByPoints = (DefinableByPoints) geometricalObject;

                // Each pair or triple (given by the object's property) together with internal objects is a definition
                // If there are not enough points, we can't do much
                if (definableByPoints.Points.Count < definableByPoints.NumberOfNeededPoints)
                    yield break;

                // Otherwise we have some definition of this type
                // Let's prepare them. First we take all the subsets (all pairs / triples) of points
                var remainingdefinitions = definableByPoints.Points.Subsets(definableByPoints.NumberOfNeededPoints)
                    // For each all the needed definition points
                    .Select(points => points.SelectMany(point => ObjectWithItsInternalObjects(point.ConfigurationObject)).ToSet());

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

        #endregion
    }
}