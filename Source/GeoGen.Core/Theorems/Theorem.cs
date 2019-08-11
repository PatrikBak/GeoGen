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
        #region Public static properties

        /// <summary>
        /// Gets the single instance of the equality comparer of two theorems that uses the 
        /// <see cref="AreTheoremsEquivalent(Theorem, Theorem)"/> method and a constant hash code function
        /// (i.e. using it together with a hash map / hash set would make all the operations O(n)).
        /// </summary>
        public static readonly IEqualityComparer<Theorem> EquivalencyComparer = new SimpleEqualityComparer<Theorem>((t1, t2) => AreTheoremsEquivalent(t1, t2), t => 0);

        #endregion

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
        /// <param name="involvedObjects">The list of theorem objects that will be wrapped directly in particular theorem objects.</param>
        public Theorem(Configuration configuration, TheoremType type, params ConfigurationObject[] objects)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Type = type;

            // Create involved objects
            InvolvedObjects = objects.Select(configurationObject => configurationObject.ObjectType switch
                {
                    // Point case
                    ConfigurationObjectType.Point => new PointTheoremObject(configurationObject) as TheoremObject,

                    // Line case
                    ConfigurationObjectType.Line => new LineTheoremObject(configurationObject),

                    // Circle case
                    ConfigurationObjectType.Circle => new CircleTheoremObject(configurationObject),

                    // Default case 
                    _ => throw new GeoGenException($"Unhandled type of configuration object: {configurationObject.ObjectType}"),
                })
                // Enumerate to an array
                .ToArray();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Finds out if this theorem can be stated without explicitly mentioning
        /// some of its internal configuration objects.
        /// </summary>
        /// <returns>true, if it cannot be stated in a smaller configuration; false otherwise.</returns>
        public bool CanBeStatedInSmallerConfiguration()
        {
            // Now we're finally ready to combine all possible definitions of particular objects
            // First take distinct involved objects
            return InvolvedObjects
                // For each object find all the definitions
                .Select(theoremObject => theoremObject.GetAllDefinitions())
                // Combine them into a single one in every possible ways
                .Combine()
                // We have needless objects if and only if there is a definition
                // containing fewer objects than we're expecting
                .Any(definition => definition.Flatten().Distinct().Count() < Configuration.AllObjects.Count);
        }

        /// <summary>
        /// Recreates the theorem by applying a given mapping of the inner configuration objects.
        /// Every <see cref="ConfigurationObject"/> internally contained in this theorem must be
        /// present in the mapping. If the mapping cannot be done (for example because 2 points
        /// making a line are mapped to the same point), then null is returned.
        /// </summary>
        /// <param name="mapping">The dictionary representing the mapping.</param>
        /// <returns>The remapped theorem, or null, if the mapping cannot be done.</returns>
        public Theorem Remap(Dictionary<ConfigurationObject, ConfigurationObject> mapping)
        {
            // Remap objects, but only if none of them is null
            var remappedObjects = InvolvedObjects.SelectIfNotDefault(o => o.Remap(mapping));

            // If the remapped objects are null, then the theorem cannot be remapped
            if (remappedObjects == null)
                return null;

            // Otherwise construct the remapped theorem
            return new Theorem(Configuration, Type, remappedObjects);
        }

        /// <summary>
        /// Determines if two theorems are equivalent, i.e. if they have the same type and geometrically
        /// represent the same statements in the same geometric situation. The way this is determined
        /// depends on their type, because different types have different semantics of their involved 
        /// theorem objects.
        /// </summary>
        /// <param name="theorem">The theorem.</param>
        /// <returns>true, if they are equivalent; false otherwise.</returns>
        public bool IsEquivalentTo(Theorem theorem) => AreTheoremsEquivalent(this, theorem);

        #endregion

        #region Public static methods

        /// <summary>
        /// Derives theorem from a list of flattened theorem objects. The idea is to accommodate the fact
        /// that <see cref="TheoremType.EqualAngles"/> and <see cref="TheoremType.EqualLineSegments"/>
        /// require <see cref="AngleTheoremObject"/> and <see cref="LineSegmentTheoremObject"/>, which
        /// internally consist of two lines and two points, i.e. their 'flattened' version have 4 lines
        /// and 4 points.
        /// </summary>
        /// <param name="configuration">The configuration in which the theorem holds.</param>
        /// <param name="type">The type of the theorem.</param>
        /// <param name="involvedObjects">The list of flattened theorem objects that this theorem is about.</param>
        /// <returns>The derived theorem.</returns>
        public static Theorem DeriveFromFlattenedObjects(Configuration configuration, TheoremType type, IReadOnlyList<TheoremObject> flattenedObjects)
        {
            // Prepare the final theorem objects
            IReadOnlyList<TheoremObject> finalTheoremObjects;

            // Switch based on the theorem type
            switch (type)
            {
                // The cases where objects are simply
                case TheoremType.CollinearPoints:
                case TheoremType.ConcyclicPoints:
                case TheoremType.ParallelLines:
                case TheoremType.PerpendicularLines:
                case TheoremType.LineTangentToCircle:
                case TheoremType.TangentCircles:
                case TheoremType.ConcurrentObjects:

                    // In those we simply take all created objects
                    finalTheoremObjects = flattenedObjects;

                    break;

                // Case with angles
                case TheoremType.EqualAngles:

                    // Here the first two lines make one angle and the next one the other one
                    finalTheoremObjects = new TheoremObject[]
                    {
                        new AngleTheoremObject((LineTheoremObject)flattenedObjects[0], (LineTheoremObject)flattenedObjects[1]),
                        new AngleTheoremObject((LineTheoremObject)flattenedObjects[2], (LineTheoremObject)flattenedObjects[3])
                    };

                    break;

                // Case with line segments
                case TheoremType.EqualLineSegments:

                    // Here the first two points make one line segment and the next one the other one
                    finalTheoremObjects = new TheoremObject[]
                    {
                        new LineSegmentTheoremObject((PointTheoremObject)flattenedObjects[0], (PointTheoremObject)flattenedObjects[1]),
                        new LineSegmentTheoremObject((PointTheoremObject)flattenedObjects[2], (PointTheoremObject)flattenedObjects[3])
                    };

                    break;

                // Default case
                default:
                    throw new GeoGenException($"Unhandled type of theorem: {type}");
            }

            // Create a new theorem using the found objects
            return new Theorem(configuration, type, finalTheoremObjects);
        }

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

            // Get the instance of the equality comparer of theorem objects
            var comparer = TheoremObject.EquivalencyComparer;

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

                // The cases where we have two objects
                case TheoremType.ParallelLines:
                case TheoremType.PerpendicularLines:
                case TheoremType.TangentCircles:
                case TheoremType.LineTangentToCircle:
                case TheoremType.EqualAngles:
                case TheoremType.EqualLineSegments:

                    // Check if the sets of their internal objects are equal
                    return theorem1.InvolvedObjects.ToSet(comparer).SetEquals(theorem2.InvolvedObjects);

                // If something else
                default:
                    throw new GeoGenException($"Unhandled type of theorem: {theorem1.Type}");
            }
        }

        #endregion

        #region To String

        /// <summary>
        /// Converts the theorem to a string. 
        /// NOTE: This method is used only for debugging purposes.
        /// </summary>
        /// <returns>A human-readable string representation of the configuration.</returns>
        public override string ToString() => $"{Type}: {InvolvedObjects.ToJoinedString()}";

        #endregion
    }
}