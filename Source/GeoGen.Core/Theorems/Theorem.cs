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

        #region Private properties

        /// <summary>
        /// The list of <see cref="InvolvedObjects"/> excluding equal ones.
        /// </summary>
        public IReadOnlyList<TheoremObject> DistinctObjects { get; }

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
            DistinctObjects = InvolvedObjects.Distinct().ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Theorem"/> object.
        /// </summary>
        /// <param name="configuration">The configuration in which the theorem holds.</param>
        /// <param name="type">The type of the theorem.</param>
        /// <param name="involvedObjects">The list of theorem objects that this theorem is about.</param>
        public Theorem(Configuration configuration, TheoremType type, params TheoremObject[] involvedObjects)
            : this(configuration, type, (IReadOnlyList<TheoremObject>) involvedObjects)
        {
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

            // Set distinct objects
            DistinctObjects = InvolvedObjects;
        }

        #endregion

        #region Public methods

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
            // Helper function that tries to map an object
            ConfigurationObject Map(ConfigurationObject configurationObject) =>
                // Try to get it from the mapping
                mapping.GetOrDefault(configurationObject)
                // If it can't be done, make the developer aware
                ?? throw new GeoGenException("Cannot create a remapped theorem, because the passed mapping doesn't contain its object.");

            // Use other function to do the job. In that case we just need to remap a single theorem object
            return Remap(theoremObject =>
            {
                // Switch based on its type
                switch (theoremObject.Type)
                {
                    // If it's a point, then we just remap the internal object
                    case ConfigurationObjectType.Point:
                        return new TheoremPointObject(Map(theoremObject.ConfigurationObject));

                    // If we have a line or circle...
                    case ConfigurationObjectType.Line:
                    case ConfigurationObjectType.Circle:

                        // We know the object has points
                        var objectWithPoints = (TheoremObjectWithPoints) theoremObject;

                        // We need to remap them
                        var points = objectWithPoints.Points.Select(Map).ToSet();

                        // If there is no internal object and not enough points, the mapping couldn't be done
                        if (theoremObject.ConfigurationObject == null && points.Count < objectWithPoints.NumberOfNeededPoints)
                            return null;

                        // We need to remap the internal object as well, if it's present
                        var internalObject = theoremObject.ConfigurationObject == null ? null : Map(theoremObject.ConfigurationObject);

                        // And we're done
                        return new TheoremObjectWithPoints(theoremObject.Type, internalObject, points);

                    // Otherwise make the developer aware
                    default:
                        throw new GeoGenException($"Unhandled theorem object type: {theoremObject.Type}");
                }
            });
        }

        /// <summary>
        /// Remaps this theorem using a custom function for remapping <see cref="TheoremObject"/>.
        /// If this function returns null for some theorem object, then the whole theorem is not remapped.
        /// This function makes sure that objects that had the same instances will have them
        /// even after the remapping.
        /// </summary>
        /// <param name="theoremObjectRemapper">The remapping function for theorem objects.</param>
        /// <returns>The remapped theorem, if it can be done; null otherwise.</returns>
        public Theorem Remap(Func<TheoremObject, TheoremObject> theoremObjectRemapper)
        {
            // Remap all distinct theoremObjects, but only if none of them is null
            var remappedDistinctObjects = DistinctObjects.SelectIfNotDefault(theoremObjectRemapper);

            // If the remapped objects are null, then the theorem cannot be remapped
            if (remappedDistinctObjects == null)
                return null;

            // If none two were equal, we can return the theorem directly
            if (remappedDistinctObjects.Count == InvolvedObjects.Count)
                return new Theorem(Configuration, Type, remappedDistinctObjects);

            // Otherwise we make a dictionary mapping objects to their remapped version
            var mappingDictionary = DistinctObjects.ZipToDictionary(remappedDistinctObjects);

            // Use it to create remapped involved objects
            var remappedInvolvedObjects = InvolvedObjects.Select(o => mappingDictionary[o]).ToList();

            // And finally create the theorem
            return new Theorem(Configuration, Type, remappedInvolvedObjects);
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

                    // This boring condition should do the job
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