using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// A helper class that keeps track of <see cref="TheoremType.Incidence"/> and  <see cref="TheoremType.EqualObjects"/> 
    /// theorems to that at each point we know about every incidence and equality that can be inferred.
    /// </summary>
    public class EqualityAndIncidenceTracker
    {
        #region Private 

        /// <summary>
        /// The dictionary mapping objects to the sets of objects equal to them.
        /// </summary>
        private readonly Dictionary<ConfigurationObject, HashSet<ConfigurationObject>> _equalObjects;

        /// <summary>
        /// The dictionary mapping lines or circle to points that contain it.
        /// </summary>
        private readonly Dictionary<ConfigurationObject, HashSet<ConfigurationObject>> _lineOrCircleToItsPoints;

        /// <summary>
        /// The dictionary mapping points to lines or circle that contain it.
        /// </summary>
        private readonly Dictionary<ConfigurationObject, HashSet<ConfigurationObject>> _pointToLinesAndCirles;

        /// <summary>
        /// The set of all the objects in appear either in an incidence of equality.
        /// </summary>
        private readonly HashSet<ConfigurationObject> _allObjects;

        #endregion

        #region Public properties

        /// <summary>
        /// The enumeration of all the objects in appear either in an incidence of equality.
        /// </summary>
        public IEnumerable<ConfigurationObject> AllObjects => _allObjects;

        /// <summary>
        /// The enumeration of all incidences that can be concluded.
        /// </summary>
        public IEnumerable<(ConfigurationObject point, ConfigurationObject lineOrCircle)> Incidences
            // For each point take every line / circle containing it
            => _pointToLinesAndCirles.SelectMany(pair => pair.Value.Select(value => (pair.Key, value)));

        /// <summary>
        /// The enumeration of all groups of equal objects with at least two objects.
        /// </summary>
        public IEnumerable<HashSet<ConfigurationObject>> EqualityGroups => _equalObjects.Values.Distinct();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualityAndIncidenceTracker"/> class.
        /// </summary>
        public EqualityAndIncidenceTracker()
        {
            // Initialize the collections
            _equalObjects = new Dictionary<ConfigurationObject, HashSet<ConfigurationObject>>();
            _allObjects = new HashSet<ConfigurationObject>();
            _lineOrCircleToItsPoints = new Dictionary<ConfigurationObject, HashSet<ConfigurationObject>>();
            _pointToLinesAndCirles = new Dictionary<ConfigurationObject, HashSet<ConfigurationObject>>();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Marks the given equality to be tracked. This call might cause changes of the
        /// incidences that use these objects.
        /// </summary>
        /// <param name="theorem">The equality theorem.</param>
        public void MarkEquality(Theorem theorem)
        {
            // Get the objects
            var object1 = ((BaseTheoremObject)theorem.InvolvedObjectsList[0]).ConfigurationObject;
            var object2 = ((BaseTheoremObject)theorem.InvolvedObjectsList[1]).ConfigurationObject;

            // Make sure they're marked
            _allObjects.Add(object1, object2);

            // Find the group for the objects
            var group1 = FindEqualityGroup(object1);
            var group2 = FindEqualityGroup(object2);

            // If they are the same, we won't do more
            if (group1 == group2)
                return;

            // Prepare the list of new equalities that will be used to alter the current incidences
            var newEqulities = group1.CombinedWith(group2).ToList();

            // We're to add everything to the first group
            // If it's new, make sure it's in the dictionary
            if (!_equalObjects.ContainsKey(object1))
                _equalObjects.Add(object1, group1);

            // Make sure the objects from the second group have now the first one
            group2.ForEach(secondGroupObject => _equalObjects[secondGroupObject] = group1);

            // Make sure the objects from the second group are merged to the first one
            group1.UnionWith(group2);

            // Handle the new equalities to derive incidences
            foreach (var pair in newEqulities)
            {
                // Deconstruct manually
                object1 = pair.Item1;
                object2 = pair.Item2;

                // Switch based on the type
                switch (object1.ObjectType)
                {
                    // Point
                    case ConfigurationObjectType.Point:

                        // Get the particular sets
                        var linesOrCircles1 = FindLinesOrCircles(object1);
                        var linesOrCircles2 = FindLinesOrCircles(object2);

                        // Merge them
                        linesOrCircles1.UnionWith(linesOrCircles2);
                        linesOrCircles2.UnionWith(linesOrCircles1);

                        // Make sure they know about the points too
                        linesOrCircles1.Concat(linesOrCircles2).ForEach(lineOrCircle => FindPoints(lineOrCircle).Add(object1, object2));

                        break;

                    // Line or circle
                    case ConfigurationObjectType.Line:
                    case ConfigurationObjectType.Circle:

                        // Get the particular sets
                        var points1 = FindPoints(object1);
                        var points2 = FindPoints(object2);

                        // Merge them
                        points1.UnionWith(points2);
                        points2.UnionWith(points1);

                        // Make sure they know about the lines too
                        points1.Concat(points2).ForEach(point => FindLinesOrCircles(point).Add(object1, object2));

                        break;

                    // Default case
                    default:
                        throw new TheoremProverException($"Unhandled type of configuration object: {object1.ObjectType}");
                }
            }
        }

        /// <summary>
        /// Marks the given incidence to be tracked. This call might find more
        /// incidences by combining this one with equalities.
        /// </summary>
        /// <param name="theorem">The incidence theorem.</param>
        public void MarkIncidence(Theorem theorem)
        {
            // Get the objects
            var object1 = ((BaseTheoremObject)theorem.InvolvedObjectsList[0]).ConfigurationObject;
            var object2 = ((BaseTheoremObject)theorem.InvolvedObjectsList[1]).ConfigurationObject;

            // Make sure they're marked
            _allObjects.Add(object1, object2);

            // Find which one is which
            var point = object1.ObjectType == ConfigurationObjectType.Point ? object1 : object2;
            var lineOrCircle = object1 == point ? object2 : object1;

            // If this incidence is already there, don't do more
            if (_pointToLinesAndCirles.GetOrDefault(point)?.Contains(lineOrCircle) ?? false)
                return;

            // Otherwise we need to figure out the other new incidences that this one brought
            // Get the group for the point and the line/circle
            var pointGroup = FindEqualityGroup(point);
            var lineGroup = FindEqualityGroup(lineOrCircle);

            // Each combination is a new incidence
            pointGroup.CombinedWith(lineGroup).ToList().ForEach(pair =>
            {
                // Get the point and line/circle
                point = pair.Item1;
                lineOrCircle = pair.Item2;

                // Mark the incidence in the dictionary mapping lines/circles to their points
                _lineOrCircleToItsPoints.GetOrAdd(lineOrCircle, () => new HashSet<ConfigurationObject>()).Add(point);

                // Mark the incidence in the dictionary mapping points to their lines/circles
                _pointToLinesAndCirles.GetOrAdd(point, () => new HashSet<ConfigurationObject>()).Add(lineOrCircle);
            });
        }

        /// <summary>
        /// Finds the set of the objects equal to a given one.
        /// </summary>
        /// <param name="configurationObject">The object.</param>
        /// <returns>The set of equal objects.</returns>
        public HashSet<ConfigurationObject> FindEqualityGroup(ConfigurationObject configurationObject)
            // Either the object is in the dictionary or we create a new set
            => _equalObjects.GetOrDefault(configurationObject) ?? new HashSet<ConfigurationObject> { configurationObject };

        /// <summary>
        /// Finds out if two objects are equal.
        /// </summary>
        /// <param name="object1">The first object.</param>
        /// <param name="object2">The second object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public bool AreEqual(ConfigurationObject object1, ConfigurationObject object2)
            // Either they are equal by definition
            => object1.Equals(object2)
                // Or they both have a group
                || _equalObjects.ContainsKey(object1) && _equalObjects.ContainsKey(object2)
                // And these groups are identical
                && _equalObjects[object1] == _equalObjects[object2];

        /// <summary>
        /// Finds all the lines and circles passing through a given point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The lines and circles passing through the point.</returns>
        public HashSet<ConfigurationObject> FindLinesOrCircles(ConfigurationObject point)
            // Either the object is in the dictionary or we create a new set
            => _pointToLinesAndCirles.GetOrDefault(point) ?? new HashSet<ConfigurationObject>();

        /// <summary>
        /// Finds all points that lie on a given line or circle.
        /// </summary>
        /// <param name="lineOrCircle">The line or circle.</param>
        /// <returns>The points lying on the line or circle.</returns>
        public HashSet<ConfigurationObject> FindPoints(ConfigurationObject lineOrCircle)
            // Either the object is in the dictionary or we create a new set
            => _lineOrCircleToItsPoints.GetOrDefault(lineOrCircle) ?? new HashSet<ConfigurationObject>();

        #endregion
    }
}