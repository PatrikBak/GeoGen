using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// A helper class that keeps track of <see cref="Incidence"/> and  <see cref="EqualObjects"/> 
    /// theorems to that at each point we know about every incidence and equality that can be inferred.
    /// </summary>
    public class EqualityAndIncidenceTracker
    {
        #region Private fields

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
        private readonly Dictionary<ConfigurationObject, HashSet<ConfigurationObject>> _pointToLinesAndCircles;

        /// <summary>
        /// The set of all the objects in appear either in an incidence of equality.
        /// </summary>
        private readonly HashSet<ConfigurationObject> _allObjects;

        #endregion

        #region Public events

        /// <summary>
        /// Raised when there are new equalities derived via the transitivity from 2 or 3 already announced equalities.
        /// </summary>
        public event Action<IReadOnlyList<(IReadOnlyList<Theorem> usedEqualities, Theorem derivedEquality)>> NewEqualities = _ => { };

        /// <summary>
        /// Raised when there are new incidences derived by exchanging objects of an old one using 1 or 2 equalities.
        /// </summary>
        public event Action<IReadOnlyList<(Theorem oldIncidence, IReadOnlyList<Theorem> usedEqualities, Theorem newIncidence)>> NewIncidences = _ => { };

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
            _pointToLinesAndCircles = new Dictionary<ConfigurationObject, HashSet<ConfigurationObject>>();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Marks the given equality to be tracked. This call might cause changes of the
        /// incidences that use these objects.
        /// </summary>
        /// <param name="equalObjectsTheorem">The equality theorem.</param>
        public void MarkEquality(Theorem equalObjectsTheorem)
        {
            // Get the objects
            var object1 = ((BaseTheoremObject)equalObjectsTheorem.InvolvedObjectsList[0]).ConfigurationObject;
            var object2 = ((BaseTheoremObject)equalObjectsTheorem.InvolvedObjectsList[1]).ConfigurationObject;

            // Make sure they're marked
            _allObjects.Add(object1, object2);

            // Find the group for the objects
            var group1 = FindEqualityGroup(object1);
            var group2 = FindEqualityGroup(object2);

            // If they are the same, we won't do more
            if (group1 == group2)
                return;

            // Prepare the list of new equalities that will be used to alter the current incidences
            var newEqualities = group1.CombinedWith(group2).ToList();

            // We're to add everything to the first group
            // If it's new, make sure it's in the dictionary
            if (!_equalObjects.ContainsKey(object1))
                _equalObjects.Add(object1, group1);

            // Make sure the objects from the second group have now the first one
            group2.ForEach(secondGroupObject => _equalObjects[secondGroupObject] = group1);

            // Make sure the objects from the second group are merged to the first one
            group1.UnionWith(group2);

            // Prepare the equality inferences to be announced via the event
            var newDerivedEqualities = new List<(IReadOnlyList<Theorem> usedEqualities, Theorem derivedEquality)>();

            // Prepare the incidence inferences to be announced via the event
            var newDerivedIncidences = new List<(Theorem oldIncidence, IReadOnlyList<Theorem> usedEqualities, Theorem newIncidence)>();

            // Handle the new equalities to derive incidences
            foreach (var newEquality in newEqualities)
            {
                // Deconstruct
                var (newEqualityObject1, newEqualityObject2) = newEquality;

                // If this is not our equality...
                if (!object1.Equals(newEqualityObject1) || !object2.Equals(newEqualityObject2))
                {
                    // Then it was derived from some actual equality
                    // Create the derived equality theorem
                    var derivedEquality = new Theorem(EqualObjects, newEqualityObject1, newEqualityObject2);

                    // New need to create the used equalities
                    var usedEqualities = new List<Theorem> { equalObjectsTheorem };

                    // If there was used an equality within the first group, add it
                    if (!object1.Equals(newEqualityObject1))
                        usedEqualities.Add(new Theorem(EqualObjects, object1, newEqualityObject1));

                    // If there was used an equality within the second group, add it
                    if (!object2.Equals(newEqualityObject2))
                        usedEqualities.Add(new Theorem(EqualObjects, object2, newEqualityObject2));

                    // Add the inferred quality to be announced later
                    newDerivedEqualities.Add((usedEqualities, derivedEquality));
                }

                // Switch based on the type
                switch (newEqualityObject1.ObjectType)
                {
                    // Point
                    case ConfigurationObjectType.Point:
                    {
                        // Get the particular sets
                        var linesOrCircles1 = FindLinesOrCircles(newEqualityObject1);
                        var linesOrCircles2 = FindLinesOrCircles(newEqualityObject2);

                        // Prepare the discovered incidences of the first point
                        var discoveredIncidences = linesOrCircles1.Select(lineOrCircle =>
                                // The current incidence
                                (oldIncidence: new Theorem(Incidence, newEqualityObject1, lineOrCircle),
                                // The equality
                                equalities: (IReadOnlyList<Theorem>)new[] { equalObjectsTheorem },
                                // The derived incidence
                                newIncidence: new Theorem(Incidence, newEqualityObject2, lineOrCircle)))
                            // Merge them with the incidences of the second point
                            .Concat(linesOrCircles2.Select(lineOrCircle =>
                                // The current incidence
                                (oldIncidence: new Theorem(Incidence, newEqualityObject2, lineOrCircle),
                                // The equality
                                equalities: (IReadOnlyList<Theorem>)new[] { equalObjectsTheorem },
                                // The derived incidence
                                newIncidence: new Theorem(Incidence, newEqualityObject1, lineOrCircle))))
                            // Enumerate
                            .ToArray();

                        // Merge them
                        linesOrCircles1.UnionWith(linesOrCircles2);
                        linesOrCircles2.UnionWith(linesOrCircles1);

                        // Make sure both of them 
                        linesOrCircles1.Concat(linesOrCircles2)
                            // Know about the points too
                            .ForEach(lineOrCircle => FindPoints(lineOrCircle).Add(newEqualityObject1, newEqualityObject2));

                        // Add the discovered incidences to be announced later
                        newDerivedIncidences.AddRange(discoveredIncidences);

                        break;
                    }

                    // Line or circle
                    case ConfigurationObjectType.Line:
                    case ConfigurationObjectType.Circle:
                    {
                        // Get the particular sets
                        var points1 = FindPoints(newEqualityObject1);
                        var points2 = FindPoints(newEqualityObject2);

                        // Prepare the discovered incidences of the first line/circle
                        var discoveredIncidences = points1.Select(point =>
                                // The current incidence
                                (oldIncidence: new Theorem(Incidence, newEqualityObject1, point),
                                // The equality
                                equalities: (IReadOnlyList<Theorem>)new[] { equalObjectsTheorem },
                                // The derived incidence
                                newIncidence: new Theorem(Incidence, newEqualityObject2, point)))
                            // Merge them with the incidences of the second line/circle
                            .Concat(points2.Select(point =>
                                // The current incidence
                                (oldIncidence: new Theorem(Incidence, newEqualityObject2, point),
                                // The equality
                                equalities: (IReadOnlyList<Theorem>)new[] { equalObjectsTheorem },
                                // The derived incidence
                                newIncidence: new Theorem(Incidence, newEqualityObject1, point))))
                            // Enumerate
                            .ToArray();

                        // Merge them
                        points1.UnionWith(points2);
                        points2.UnionWith(points1);

                        // Make sure both of them 
                        points1.Concat(points2)
                            // Know about the points too
                            .ForEach(point => FindLinesOrCircles(point).Add(newEqualityObject1, newEqualityObject2));

                        // Add the discovered incidences to be announced later
                        newDerivedIncidences.AddRange(discoveredIncidences);

                        break;
                    }

                    // Default case
                    default:
                        throw new TheoremProverException($"Unhandled type of configuration object: {newEqualityObject1.ObjectType}");
                }

                // Announce the derived equalities
                if (newDerivedEqualities.Any())
                    NewEqualities(newDerivedEqualities);

                // Announce the derived incidences
                if (newDerivedIncidences.Any())
                    NewIncidences(newDerivedIncidences);
            }
        }

        /// <summary>
        /// Marks the given incidence to be tracked. This call might find more
        /// incidences by combining this one with equalities.
        /// </summary>
        /// <param name="incidenceTheorem">The incidence theorem.</param>
        public void MarkIncidence(Theorem incidenceTheorem)
        {
            // Get the objects
            var object1 = ((BaseTheoremObject)incidenceTheorem.InvolvedObjectsList[0]).ConfigurationObject;
            var object2 = ((BaseTheoremObject)incidenceTheorem.InvolvedObjectsList[1]).ConfigurationObject;

            // Make sure they're marked
            _allObjects.Add(object1, object2);

            // Find which one is which
            var point = object1.ObjectType == ConfigurationObjectType.Point ? object1 : object2;
            var lineOrCircle = object1 == point ? object2 : object1;

            // If this incidence is already there, don't do more
            if (_pointToLinesAndCircles.GetOrDefault(point)?.Contains(lineOrCircle) ?? false)
                return;

            // Otherwise we need to figure out the other new incidences that this one brought
            // Get the group for the point and the line/circle
            var pointGroup = FindEqualityGroup(point);
            var lineOrCircleGroup = FindEqualityGroup(lineOrCircle);

            // Prepare the incidence inferences to be announced via the event
            var newDerivedIncidences = new List<(Theorem oldIncidence, IReadOnlyList<Theorem> usedEqualities, Theorem newIncidence)>();

            // Each combination is a new incidence
            pointGroup.CombinedWith(lineOrCircleGroup).ToList().ForEach(pair =>
            {
                // Deconstruct
                var (newIncidencePoint, newIncidenceLineOrCircle) = pair;

                // Create the new incidence
                var newIncidence = new Theorem(Incidence, newIncidencePoint, newIncidenceLineOrCircle);

                // Mark the incidence in the dictionary mapping lines/circles to their points
                _lineOrCircleToItsPoints.GetOrAdd(lineOrCircle, () => new HashSet<ConfigurationObject>()).Add(point);

                // Mark the incidence in the dictionary mapping points to their lines/circles
                _pointToLinesAndCircles.GetOrAdd(point, () => new HashSet<ConfigurationObject>()).Add(lineOrCircle);

                // If this is not the incidence being added...
                if (!point.Equals(newIncidencePoint) || !lineOrCircle.Equals(newIncidenceLineOrCircle))
                {
                    // Create the list of used equalities
                    var usedEqualities = new List<Theorem>();

                    // If the point was replaced via an equality, mark it
                    if (!point.Equals(newIncidencePoint))
                        usedEqualities.Add(new Theorem(EqualObjects, point, newIncidencePoint));

                    // If the line or circle was replaced via an equality, mark it
                    if (!lineOrCircle.Equals(newIncidenceLineOrCircle))
                        usedEqualities.Add(new Theorem(EqualObjects, lineOrCircle, newIncidenceLineOrCircle));

                    // Mark the derived incidence
                    newDerivedIncidences.Add((incidenceTheorem, usedEqualities, newIncidence));
                }
            });

            // Announce the derived incidences
            if (newDerivedIncidences.Any())
                NewIncidences(newDerivedIncidences);
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
        /// Finds all the lines and circles passing through a given point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>The lines and circles passing through the point.</returns>
        public HashSet<ConfigurationObject> FindLinesOrCircles(ConfigurationObject point)
            // Either the object is in the dictionary or we create a new set
            => _pointToLinesAndCircles.GetOrDefault(point) ?? new HashSet<ConfigurationObject>();

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