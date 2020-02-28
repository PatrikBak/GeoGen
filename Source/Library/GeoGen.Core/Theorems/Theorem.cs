using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.Core.TheoremType;

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
        /// Gets the type of the theorem.
        /// </summary>
        public TheoremType Type { get; }

        /// <summary>
        /// Gets the set of theorem objects that this theorem is about.
        /// </summary>
        public IReadOnlyHashSet<TheoremObject> InvolvedObjects { get; }

        /// <summary>
        /// Gets the <see cref="InvolvedObjects"/> as a list.
        /// </summary>
        public IReadOnlyList<TheoremObject> InvolvedObjectsList { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Theorem"/> object.
        /// </summary>
        /// <param name="type">The type of the theorem.</param>
        /// <param name="involvedObjects">The theorem objects that this theorem is about.</param>
        public Theorem(TheoremType type, IEnumerable<TheoremObject> involvedObjects)
        {
            Type = type;
            InvolvedObjects = involvedObjects?.ToReadOnlyHashSet() ?? throw new ArgumentNullException(nameof(involvedObjects));

            // Get the list version of the objects
            InvolvedObjectsList = InvolvedObjects.ToList();

            // Ensure the number of objects is correct
            if (InvolvedObjects.Count != type.GetNumberOfNeededObjects())
                throw new GeoGenException($"Cannot create theorem of type '{type}', incorrect number of objects: {InvolvedObjects.Count}");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Theorem"/> object.
        /// </summary>
        /// <param name="type">The type of the theorem.</param>
        /// <param name="involvedObjects">The list of theorem objects that will be wrapped directly in particular theorem objects.</param>
        public Theorem(TheoremType type, params ConfigurationObject[] objects)
        {
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

                // Unhandled cases
                _ => throw new GeoGenException($"Unhandled value of {nameof(ConfigurationObjectType)}: {configurationObject.ObjectType}"),
            })
            // Enumerate to an array
            .ToReadOnlyHashSet();

            // Get the list version of the objects
            InvolvedObjectsList = InvolvedObjects.ToList();

            // Ensure the number of objects is correct
            if (InvolvedObjects.Count != type.GetNumberOfNeededObjects())
                throw new GeoGenException($"Cannot create theorem of type '{type}', incorrect number of objects: {InvolvedObjects.Count}");
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Finds the objects of a given configuration that are not needed to state this theorem.
        /// </summary>
        /// <param name="configuration">The configuration with respect to which we're finding unnecessary objects.</param>
        /// <returns>The unnecessary objects with regards to this theorem.</returns>
        public IReadOnlyHashSet<ConfigurationObject> GetUnnecessaryObjects(Configuration configuration)
        {
            // From the objects of the configuration
            return configuration.AllObjects
                // Exclude all the inner objects of the theorem objects
                .Except(GetInnerConfigurationObjects().GetDefiningObjects())
                // Enumerate
                .ToReadOnlyHashSet();
        }

        /// <summary>
        /// Finds the inner configuration objects of the theorem objects.
        /// </summary>
        /// <returns>The inner configuration objects of all theorems objects</returns>
        public IReadOnlyList<ConfigurationObject> GetInnerConfigurationObjects()
        {
            // Simply merge all the inner objects of all the theorem objects
            return InvolvedObjects.SelectMany(theoremObject => theoremObject.GetInnerConfigurationObjects()).Distinct().ToArray();
        }

        /// <summary>
        /// Recreates the theorem by applying a given mapping of the inner configuration objects.
        /// Every <see cref="ConfigurationObject"/> internally contained in this theorem must be
        /// present in the mapping. If the mapping cannot be done (for example because 2 points
        /// making a line are mapped to the same point), then null is returned.
        /// </summary>
        /// <param name="mapping">The dictionary representing the mapping.</param>
        /// <param name="flattenObjectsFromPoints">Indicates whether explicit objects LineFromPoints or Circumcircle should be made implicit.</param>
        /// <returns>The remapped theorem, or null, if the mapping cannot be done.</returns>
        public Theorem Remap(IReadOnlyDictionary<ConfigurationObject, ConfigurationObject> mapping, bool flattenObjectsFromPoints = false)
        {
            // Remap objects
            var remappedObjects = InvolvedObjects
                // Only if none of them is null
                .SelectIfNotDefault(involvedObject => involvedObject.Remap(mapping, flattenObjectsFromPoints))?
                // Take distinct ones
                .Distinct()
                // Enumerate
                .ToList();

            // If the remapped objects are null, then the theorem cannot be remapped
            if (remappedObjects == null)
                return null;

            // If the number of objects doesn't match, then the mapping is incorrect
            if (remappedObjects.Count != Type.GetNumberOfNeededObjects())
                return null;

            // Otherwise construct the remapped theorem
            return new Theorem(Type, remappedObjects);
        }

        /// <summary>
        /// Infers new theorems from symmetry of a given configuration where these theorems hold.
        /// </summary>
        /// <param name="configuration">The configuration where the theorem holds.</param>
        /// <returns>The enumerable of theorems that are distinct from this one and inferable from symmetry.</returns>
        public IEnumerable<Theorem> InferTheoremsFromSymmetry(Configuration configuration)
            // Take all mappings that keep the configuration the same
            => configuration.GetSymmetryMappings()
                // Remap the theorem in them
                .Select(mapping => Remap(mapping))
                // Take distinct results with the exclusion of this theorem
                .Except(this.ToEnumerable());


        #endregion

        #region Public static methods

        /// <summary>
        /// Derives theorem from a list of flattened theorem objects. The idea is to accommodate the fact
        /// that <see cref="EqualAngles"/> and <see cref="EqualLineSegments"/>
        /// require <see cref="AngleTheoremObject"/> and <see cref="LineSegmentTheoremObject"/>, which
        /// internally consist of two lines and two points, i.e. their 'flattened' version have 4 lines
        /// and 4 points.
        /// </summary>
        /// <param name="type">The type of the theorem.</param>
        /// <param name="flattenedObjects">The list of flattened theorem objects that this theorem is about.</param>
        /// <returns>The derived theorem.</returns>
        public static Theorem DeriveFromFlattenedObjects(TheoremType type, IReadOnlyList<TheoremObject> flattenedObjects)
        {
            // Prepare the final theorem objects
            IReadOnlyList<TheoremObject> finalTheoremObjects;

            // Switch based on the theorem type
            switch (type)
            {
                // The cases where objects are simply the ones passed
                case CollinearPoints:
                case ConcyclicPoints:
                case ConcurrentLines:
                case ParallelLines:
                case PerpendicularLines:
                case LineTangentToCircle:
                case TangentCircles:
                case ConcurrentObjects:
                case EqualObjects:
                case Incidence:

                    // In those we simply take all created objects
                    finalTheoremObjects = flattenedObjects;

                    break;

                // Case with angles
                case EqualAngles:

                    // Here the first two lines make one angle and the next one the other one
                    finalTheoremObjects = new TheoremObject[]
                    {
                        new AngleTheoremObject((LineTheoremObject)flattenedObjects[0], (LineTheoremObject)flattenedObjects[1]),
                        new AngleTheoremObject((LineTheoremObject)flattenedObjects[2], (LineTheoremObject)flattenedObjects[3])
                    };

                    break;

                // Case with line segments
                case EqualLineSegments:

                    // Here the first two points make one line segment and the next one the other one
                    finalTheoremObjects = new TheoremObject[]
                    {
                        new LineSegmentTheoremObject((PointTheoremObject)flattenedObjects[0], (PointTheoremObject)flattenedObjects[1]),
                        new LineSegmentTheoremObject((PointTheoremObject)flattenedObjects[2], (PointTheoremObject)flattenedObjects[3])
                    };

                    break;

                // Unhandled cases
                default:
                    throw new GeoGenException($"Unhandled value of {nameof(TheoremType)}: {type}");
            }

            // Create a new theorem using the found objects
            return new Theorem(type, finalTheoremObjects);
        }

        #endregion

        #region HashCode and Equals

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => (Type, InvolvedObjects).GetHashCode();

        /// <summary>
        /// Finds out if a passed object is equal to this one.
        /// </summary>
        /// <param name="otherObject">The passed object.</param>
        /// <returns>true, if they are equal; false otherwise.</returns>
        public override bool Equals(object otherObject)
        {
            // Either the references are equals
            return this == otherObject
                // Or the object is not null
                || otherObject != null
                // And it is a theorem
                && otherObject is Theorem theorem
                // And their types matches
                && Type.Equals(theorem.Type)
                // And their involved objects matches
                && InvolvedObjects.Equals(theorem.InvolvedObjects);
        }

        #endregion

        #region Debug-only to string

#if DEBUG

        /// <summary>
        /// Converts the theorem to a string. 
        /// </summary>
        /// <returns>A human-readable string representation of the theorem.</returns>
        public override string ToString() => $"{Type}: {InvolvedObjects.Select(theoremObject => theoremObject.ToString()).Ordered().ToJoinedString()}";

#endif

        #endregion
    }
}