using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents a geometric theorem that holds true in some configuration. It has a certain
    /// <see cref="TheoremType"/> and it is expressed via <see cref="TheoremObject"/>s that wrap 
    /// <see cref="ConfigurationObject"/>s. 
    /// </summary>
    public class Theorem
    {
        #region Public properties

        /// <summary>
        /// Gets the configuration in which the theorem holds. 
        /// </summary>
        public Configuration Configuration { get; }

        /// <summary>
        /// Gets the type of the theorem.
        /// </summary>
        public TheoremType Type { get; }

        /// <summary>
        /// Gets the list of theorem objects that this theorem is about.
        /// </summary>
        public IReadOnlyList<TheoremObject> InvolvedObjects { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Theorem"/> object.
        /// </summary>
        /// <param name="configuration">The configuration in which the theorem holds.</param>
        /// <param name="type">The type of the theorem.</param>
        /// <param name="involvedObjects">The list of theorem objects that this theorem is about.</param>
        public Theorem(Configuration configuration, TheoremType type, IReadOnlyList<TheoremObject> involvedObjects)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Type = type;
            InvolvedObjects = involvedObjects ?? throw new ArgumentNullException(nameof(involvedObjects));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Theorem"/> object.
        /// </summary>
        /// <param name="configuration">The configuration in which the theorem holds.</param>
        /// <param name="type">The type of the theorem.</param>
        /// <param name="involvedObjects">The list of theorem objects that will be wrapped directly in theorem objects.</param>
        public Theorem(Configuration configuration, TheoremType type, params ConfigurationObject[] objects)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Type = type;

            // Create involved objects
            InvolvedObjects = objects.Select(
                // If the current object is a point
                o => o.ObjectType == ConfigurationObjectType.Point
                // Create a point object
                ? (TheoremObject) new TheoremPointObject(o)
                // Otherwise create a line/circle object
                : new TheoremObjectWithPoints(o))
                // Enumerate to an array
                .ToArray();
        }

        #endregion

        #region Public static methods

        /// <summary>
        /// Determines if two theorems are equivalent, i.e. if they have the same type and geometrically
        /// represent the same statements in the same geometric situation. The way this is determined
        /// depends on their type, because different types have different semantics of their involved 
        /// theorem objects.
        /// </summary>
        /// <param name="theorem1">The first theorem.</param>
        /// <param name="theorem2">The second theorem.</param>
        /// <returns>true, if they are equivalent; false otherwise.</returns>
        public static bool AreTheoremsEquivalent(Theorem theorem1, Theorem theorem2)
        {
            // If they don't have the same type, then they are not equivalent
            if (theorem1.Type != theorem2.Type)
                return false;

            // If they don't have the same number of involved objects, then they are not equivalent
            if (theorem1.InvolvedObjects.Count != theorem2.InvolvedObjects.Count)
                return false;

            // Local helper function that checks if two theorem objects are equivalent
            bool AreTheoremObjectsEquivalent(TheoremObject theoremObject1, TheoremObject theoremObject2)
            {
                // Check their types
                if (theoremObject1.Type != theoremObject2.Type)
                    return false;

                // Handle the point case in which we need to have equal configuration objects
                if (theoremObject1 is TheoremPointObject)
                    return theoremObject1.ConfigurationObject == theoremObject2.ConfigurationObject;

                // Otherwise we have the line/circle case
                var lineOrCircle1 = (TheoremObjectWithPoints) theoremObject1;
                var lineOrCircle2 = (TheoremObjectWithPoints) theoremObject2;

                // If their configuration objects are defined and matches, then they are equivalent
                if (lineOrCircle1.ConfigurationObject != null && lineOrCircle1.ConfigurationObject == lineOrCircle2.ConfigurationObject)
                    return true;

                // If the number of their common points is enough to define them, then they are equivalent
                if (lineOrCircle1.Points.Intersect(lineOrCircle2.Points).Count() >= lineOrCircle1.NumberOfNeededPoints)
                    return true;

                // Otherwise we don't have enough information to say they are equivalent for sure
                return false;
            }

            // Prepare a simple comparer for hash set of theorem objects that uses our defined equivalence
            // It uses a trivial hash-code function which makes it inefficient, but at this scale it doesn't matter
            var comparer = new SimpleEqualityComparer<TheoremObject>((o1, o2) => AreTheoremObjectsEquivalent(o1, o2), o => 0);

            // We need to take care particular types since they might have specific signatures
            switch (theorem1.Type)
            {
                // The cases where the objects can be in any order
                case TheoremType.CollinearPoints:
                case TheoremType.ConcyclicPoints:
                case TheoremType.ConcurrentObjects:

                    // Prepare the number of needed common objects for theorems to be equivalent
                    var neededPoints = theorem1.Type == TheoremType.CollinearPoints || theorem1.Type == TheoremType.ConcurrentObjects ? 3 : 4;

                    // Check if there is at least the needed number of common objects
                    return theorem1.InvolvedObjects.ToSet(comparer).Intersect(theorem2.InvolvedObjects, comparer).Count() >= neededPoints;

                case TheoremType.ParallelLines:
                case TheoremType.PerpendicularLines:
                case TheoremType.TangentCircles:
                case TheoremType.LineTangentToCircle:

                    // Check if the sets of their internal objects are equal
                    return theorem1.InvolvedObjects.ToSet(comparer).SetEquals(theorem2.InvolvedObjects);

                // The cases where we basically have the signature {{x, x}, {x, x}} 
                // (i.e. objects in pair can be in any order, and the pairs itself as well)
                case TheoremType.EqualAngles:
                case TheoremType.EqualLineSegments:

                    // Get the pairs for each object
                    var obj1Set1 = new HashSet<TheoremObject>(comparer) { theorem1.InvolvedObjects[0], theorem1.InvolvedObjects[1] };
                    var obj1Set2 = new HashSet<TheoremObject>(comparer) { theorem1.InvolvedObjects[2], theorem1.InvolvedObjects[3] };
                    var obj2Set1 = new HashSet<TheoremObject>(comparer) { theorem2.InvolvedObjects[0], theorem2.InvolvedObjects[1] };
                    var obj2Set2 = new HashSet<TheoremObject>(comparer) { theorem2.InvolvedObjects[2], theorem2.InvolvedObjects[3] };

                    // Write this boring condition that should do the job
                    return obj1Set1.SetEquals(obj2Set1) && obj1Set2.SetEquals(obj2Set2) ||
                           obj1Set1.SetEquals(obj2Set2) && obj1Set2.SetEquals(obj2Set1);

                // Default case
                default:
                    throw new GeoGenException("Unhandled type");
            }
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts the theorem to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString() => $"{Type}: {string.Join(", ", InvolvedObjects)}";

        #endregion
    }
}